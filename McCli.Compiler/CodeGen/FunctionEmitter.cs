using CliKit;
using CliKit.IO;
using McCli.Compiler.IR;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ILGenerator = System.Reflection.Emit.ILGenerator;
using VariableKind = McCli.Compiler.IR.VariableKind;

namespace McCli.Compiler.CodeGen
{
	/// <summary>
	/// Generates the IL bytecode for MatLab function bodies.
	/// </summary>
	public sealed partial class FunctionEmitter
	{
		#region EmitStoreScope type
		private struct EmitStoreScope : IDisposable
		{
			private readonly FunctionEmitter instance;
			private readonly Variable variable;

			public EmitStoreScope(FunctionEmitter instance, Variable variable)
			{
				this.instance = instance;
				this.variable = variable;
			}

			public void Dispose()
			{
				instance.EndEmitStore(variable);
			}
		}
		#endregion

		#region Fields
		private readonly IR.Function declaration;
		private readonly FunctionLookup functionLookup;
		private readonly FunctionMethod method;
		private readonly MethodBodyWriter cil;
		private readonly Dictionary<Variable, VariableLocation> locals = new Dictionary<Variable, VariableLocation>();
		private readonly TemporaryLocalPool temporaryPool;
		private Label returnTargetLabel;
		#endregion

		#region Constructors
		internal FunctionEmitter(IR.Function function, MethodFactory methodFactory, FunctionLookup functionLookup)
		{
			Contract.Requires(function != null);
			Contract.Requires(methodFactory != null);
			Contract.Requires(functionLookup != null);

			this.declaration = function;
			this.functionLookup = functionLookup;
			var signature = new FunctionSignature(function.Inputs.Select(i => i.StaticRepr), function.Outputs.Select(o => o.StaticRepr));

			// Determine the method signature
			var parameterDescriptors = new List<ParameterDescriptor>();
			foreach (var input in function.Inputs)
			{
				locals.Add(input, VariableLocation.Parameter(parameterDescriptors.Count));
				parameterDescriptors.Add(new ParameterDescriptor(input.StaticCliType, ParameterAttributes.None, input.Name));
			}

			Type returnType = typeof(void);
			if (function.Outputs.Length == 1)
			{
				returnType = function.Outputs[0].StaticCliType; // 1 output, use return value
			}
			else if (function.Outputs.Length >= 2)
			{
				// 2 or more outputs, use 'out' parameters
				foreach (var output in function.Outputs)
				{
					string name = output.Name;
					if (locals.ContainsKey(output))
					{
						// inout parameter, rename not to clash with input
						name += "$out";
					}
					else
					{
						locals.Add(output, VariableLocation.Parameter(parameterDescriptors.Count));
					}

					var parameterType = output.StaticCliType.MakeByRefType();
					parameterDescriptors.Add(new ParameterDescriptor(parameterType, ParameterAttributes.Out, name));
				}
			}

			// Create the method and get its IL generator
			ILGenerator ilGenerator;
			var methodInfo = methodFactory(function.Name, parameterDescriptors, returnType, out ilGenerator);
			this.method = new FunctionMethod(methodInfo, signature);

			cil = new ILGeneratorMethodBodyWriter(ilGenerator);
			cil = new MethodBodyVerifier(new MethodBodyVerificationContext
			{
				Method = methodInfo,
				ParameterTypes = parameterDescriptors.Select(p => p.Type).ToArray(),
				ReturnType = returnType,
				HasInitLocals = true,
				MaxStackSize = ushort.MaxValue
			}, cil);
			
			temporaryPool = new TemporaryLocalPool(cil, "$temp");

			if (function.Outputs.Length == 1)
			{
				// Declare a local variable for the return value
				var output = function.Outputs[0];
				var localIndex = cil.DeclareLocal(output.StaticCliType, output.Name);
				if (!function.Inputs.Contains(output))
					locals.Add(output, VariableLocation.Local(localIndex));
			}
		}
		#endregion

		#region Properties
		public FunctionMethod Method
		{
			get { return method; }
		}
		#endregion

		#region Methods
		public static FunctionMethod Emit(IR.Function function, MethodFactory methodFactory, FunctionLookup functionLookup)
		{
			Contract.Requires(function != null);
			Contract.Requires(methodFactory != null);
			Contract.Requires(functionLookup != null);

			var emitter = new FunctionEmitter(function, methodFactory, functionLookup);
			emitter.EmitBody();
			return emitter.Method;
		}

		public void EmitBody()
		{
			returnTargetLabel = cil.CreateLabel("return");

			EmitCloneInputs();
			EmitStatements(declaration.Body);

			cil.MarkLabel(returnTargetLabel);
			if (declaration.Outputs.Length == 1)
			{
				// Load the return value
				EmitLoad(declaration.Outputs[0]);
			}
			else if (declaration.Outputs.Length >= 2)
			{
				// Copy inout parameters from their input to their outputs (since we operate only on the input)
				for (int inputIndex = 0; inputIndex < declaration.Inputs.Length; ++inputIndex)
				{
					var input = declaration.Inputs[inputIndex];
					int outputIndex = declaration.Outputs.IndexOf(input);
					if (outputIndex > -1)
					{
						cil.LoadLocal(declaration.Inputs.Length + outputIndex);
						cil.LoadLocal(inputIndex);
						cil.StoreIndirect(input.StaticCliType);
					}
				}
			}
			cil.Ret();
		}

		private void EmitCloneInputs()
		{
			foreach (var input in declaration.Inputs)
			{
				// Don't clone if we never modify the variable or if it's a scalar
				if (input.IsInitOnly || IsLiteral(input) || !input.StaticRepr.IsMValue) continue;

				using (BeginEmitStore(input))
				{
					EmitLoad(input);
					EmitCloneIfNeeded(input.StaticRepr);
				}
			}
		}

		private void EmitStatements(ImmutableArray<Statement> statements)
		{
			foreach (var statement in statements)
			{
				// Do not generate expressions producing constant primitive values
				var expression = statement as Expression;
				if (expression != null)
				{
					bool onlyProducesConstants = true;
					for (int targetIndex = 0; targetIndex < expression.TargetCount; ++targetIndex)
					{
						if (!IsLiteral(expression.GetTarget(targetIndex)))
						{
							onlyProducesConstants = false;
							break;
						}
					}

					if (onlyProducesConstants) continue;
				}

				statement.Accept(this);
			}
		}

		private bool IsLiteral(Variable variable)
		{
			return variable.ConstantValue != null && variable.StaticRepr.IsPrimitiveScalar;
		}
		
		private void EmitLoad(Variable variable)
		{
			if (IsLiteral(variable))
			{
				if (variable.ConstantValue is double)
					cil.LoadFloat64((double)variable.ConstantValue);
				else
					throw new NotImplementedException("Loading constants of type " + variable.StaticRepr);
				return;
			}

			cil.Load(GetLocation(variable));
			if (IsByRef(variable))
			{
				// Dereference to get the value
				cil.LoadIndirect(variable.StaticCliType);
			}
		}

		private bool IsByRef(Variable variable)
		{
			return declaration.Outputs.Length >= 2
				&& declaration.Outputs.Contains(variable)
				&& !declaration.Inputs.Contains(variable);
		}

		private EmitStoreScope BeginEmitStore(Variable variable)
		{
			if (IsByRef(variable)) cil.Load(GetLocation(variable));
			return new EmitStoreScope(this, variable);
		}

		private void EndEmitStore(Variable variable)
		{
			if (variable == null) return;

			if (IsByRef(variable)) cil.StoreIndirect(variable.StaticCliType);
			else cil.Store(GetLocation(variable));
		}

		private void EmitConversion(MRepr source, MRepr target)
		{
			if (source == target) return;

			if (target.IsAny)
			{
				if (source.IsMValue) return;

				if (source.IsPrimitiveScalar)
				{
					EmitBoxScalar(source.Type);
					return;
				}
			}

			if (source.Type == target.Type)
			{
				var type = source.Type;

				if (source.StructuralClass == MStructuralClass.Scalar
					&& (target.StructuralClass == MStructuralClass.Array
						|| target.StructuralClass == MStructuralClass.FullArray
						|| target.IsAny))
				{
					// Box a scalar to an array
					EmitBoxScalar(type);
					return;
				}
				else if (source.IsArray
					&& target.StructuralClass == MStructuralClass.Scalar)
				{
					// Convert an array assumed to have size 1x1 to a scalar.
					cil.Invoke(typeof(MArray<>).MakeGenericType(type.CliType).GetMethod("ToScalar"));
					return;
				}

				// Upcast
				if (source.StructuralClass.IsArray && (target.IsArray || target.IsAny)) return;
			}

			throw new NotImplementedException(
				string.Format("Conversion from {0} to {1}.", source, target));
		}

		private void EmitConversion(MRepr source, MStructuralClass target)
		{
			if (source.StructuralClass != target) EmitConversion(source, source.WithStructuralClass(target));
		}

		private void EmitBoxScalar(MType type)
		{
			cil.Invoke(typeof(MFullArray<>).MakeGenericType(type.CliType).GetMethod("CreateScalar"));
		}

		private VariableLocation GetLocation(Variable variable)
		{
			VariableLocation location;
			if (!locals.TryGetValue(variable, out location))
			{
				var localIndex = cil.DeclareLocal(variable.StaticRepr.CliType, variable.Name);
				location = VariableLocation.Local(localIndex);
				locals.Add(variable, location);
			}

			return location;
		}
		#endregion
	}
}
