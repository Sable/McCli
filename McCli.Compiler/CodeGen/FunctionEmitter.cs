﻿using CliKit;
using CliKit.Cil;
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
		private int depth;
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

			depth = -1;
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
			++depth;
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
			--depth;
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
					EmitLoadConstant((double)variable.ConstantValue, variable.StaticRepr.Class);
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

		private void EmitLoadConstant(double value, MClass mclass)
		{
			if (mclass.IsInteger)
			{
				// IntOK'd variable
				if (mclass == MClass.Int64)
				{
					Contract.Assert((long)value == value);
					cil.LoadInt64((long)value);
				}
				else if (mclass == MClass.Int32)
				{
					Contract.Assert((int)value == value);
					cil.LoadInt32((int)value);
				}
			}
			else if (mclass == MClass.Double)
			{
				cil.LoadFloat64(value);
			}
		}

		private void EmitConvert(MRepr source, MRepr target)
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
					var function = pseudoBuiltins.Lookup("ToScalar", source);
					cil.Invoke(function.Method);
					return;
				}
				else if (source.StructuralClass == MStructuralClass.IntegralRange
					&& (target.IsArray || target.IsAny))
				{
					// Integral range to array
					var function = pseudoBuiltins.Lookup("ToArray", source);
					cil.Invoke(function.Method);
					return;
				}

				// Upcast
				if (source.StructuralClass.IsArray && (target.IsArray || target.IsAny)) return;
			}
			else if (source.Class == target.Class)
			{
				if (target.IsComplex)
				{
					// Real to complex promotion

					// Convert to complex
					var function = pseudoBuiltins.Lookup("ToComplex", source);
					cil.Invoke(function.Method);

					// Change structural class if needed
					EmitConvert(function.Signature.Outputs[0], target);
				}
				else
				{
					Contract.Assert(source.IsComplex);

					// Complex to real demotion
					// We assume that this happens only in the case of variables
					// that can be either scalar or complex over their lifetime.
					var function = pseudoBuiltins.Lookup("GetRealPart", source);
					cil.Invoke(function.Method);

					// Change structural class if needed
					EmitConvert(function.Signature.Outputs[0], target);
				}
				return;
			}
			else
			{
				// Different classes
				if (source.StructuralClass == MStructuralClass.Scalar
					&& source.Class.IsInteger && source.Type.IsReal)
				{
					if (target.Class.IsFloat)
					{
						// Int to float
						if (source.Class.IsUnsignedInteger)
							cil.Instruction(Opcode.Conv_R_Un);
						else if (target.Class == MClass.Single)
							cil.Instruction(Opcode.Conv_R4);
						else if (target.Class == MClass.Double)
							cil.Instruction(Opcode.Conv_R8);
						else
							throw new NotSupportedException("Unexpected int-to-float conversion.");

						// Do the rest of the conversion (complex, structural class), if needed
						EmitConvert(new MRepr(target.Class, MStructuralClass.Scalar), target);
						return;
					}
					else if (source.Class == MClass.Int32 && target.Class == MClass.Int64)
					{
						// Int32 to Int64 (hack for IntOK)
						cil.Instruction(Opcode.Conv_I8);
						return;
					}
				}
			}

			throw new NotImplementedException(
				string.Format("Conversion from {0} to {1}.", source, target));
		}

		private void EmitLoadAndConvert(Variable variable, MRepr targetRepr)
		{
			EmitLoad(variable);
			EmitConvert(variable.StaticRepr, targetRepr);
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
