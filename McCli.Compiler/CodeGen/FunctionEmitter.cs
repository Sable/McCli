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
		private readonly IR.Function function;
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

			Contract.Assert(function.Outputs.Length <= 1);

			this.function = function;
			this.functionLookup = functionLookup;

			// Handle method parameters
			var inputDescriptorsBuilder = new ImmutableArray<ParameterDescriptor>.Builder(function.Inputs.Length);

			for (int i = 0; i < function.Inputs.Length; ++i)
			{
				var input = function.Inputs[i];
				locals.Add(input, VariableLocation.Parameter(i));
				inputDescriptorsBuilder[i] = new ParameterDescriptor(input.StaticRepr.CliType, ParameterAttributes.In, input.Name);
			}
			
			var inputDescriptors = inputDescriptorsBuilder.Complete();
			var outputType = function.Outputs.Length == 0 ? typeof(void) : function.Outputs[0].StaticRepr.CliType;

			// Create the method and get its IL generator
			ILGenerator ilGenerator;
			var methodInfo = methodFactory(function.Name, inputDescriptors, outputType, out ilGenerator);

			this.method = new FunctionMethod(methodInfo, inputDescriptors.Select(i => i.Type).ToArray(), outputType);

			cil = new ILGeneratorMethodBodyWriter(ilGenerator);
			temporaryPool = new TemporaryLocalPool(cil, "$temp");

			foreach (var output in function.Outputs)
			{
				// We might have already seen that output as an input (inout parameter)
				if (locals.ContainsKey(output)) continue;

				var localIndex = cil.DeclareLocal(output.StaticRepr.CliType, output.Name);
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

			EmitStatements(function.Body);

			cil.MarkLabel(returnTargetLabel);
			if (function.Outputs.Length == 1) EmitLoad(function.Outputs[0]);
			cil.Ret();
		}

		private void EmitStatements(ImmutableArray<Statement> statements)
		{
			foreach (var statement in statements)
				statement.Accept(this);
		}
		
		private void EmitLoad(Variable variable)
		{
			cil.Load(GetLocalLocation(variable));
		}

		private EmitStoreScope BeginEmitStore(Variable variable)
		{
			return new EmitStoreScope(this, variable);
		}

		private void EndEmitStore(Variable variable)
		{
			if (variable == null) return;

			cil.Store(GetLocalLocation(variable));
		}

		private void EmitConversion(MRepr source, MRepr target)
		{
			if (source == target) return;

			if (target.IsAny)
			{
				if (source.IsMValue) return;

				if (source.IsPrimitive && source.StructuralClass == MStructuralClass.Scalar)
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
					cil.Call(typeof(MArray<>).MakeGenericType(type.CliType).GetMethod("ToScalar"));
					return;
				}

				// Upcast
				if (source.StructuralClass.IsArray && (target.IsArray || target.IsAny)) return;
			}

			throw new NotImplementedException(
				string.Format("Conversion from {0} to {1}.", source, target));
		}

		private void EmitBoxScalar(MType type)
		{
			cil.Call(typeof(MFullArray<>).MakeGenericType(type.CliType).GetMethod("CreateScalar"));
		}

		private VariableLocation GetLocalLocation(Variable variable)
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
