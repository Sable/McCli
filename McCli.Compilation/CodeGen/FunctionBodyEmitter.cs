using CliKit;
using CliKit.IO;
using McCli.Compilation.IR;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ILGenerator = System.Reflection.Emit.ILGenerator;
using VariableKind = McCli.Compilation.IR.VariableKind;

namespace McCli.Compilation.CodeGen
{
	/// <summary>
	/// Generates the IL bytecode for MatLab function bodies.
	/// </summary>
	public sealed partial class FunctionBodyEmitter
	{
		#region EmitStoreScope type
		private struct EmitStoreScope : IDisposable
		{
			private readonly FunctionBodyEmitter instance;
			private readonly Variable variable;

			public EmitStoreScope(FunctionBodyEmitter instance, Variable variable)
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
		private readonly MethodInfo method;
		private readonly MethodBodyWriter cil;
		private readonly Dictionary<Variable, VariableLocation> locals = new Dictionary<Variable, VariableLocation>();
		private readonly TemporaryLocalPool temporaryPool;
		private Label returnTargetLabel;
		#endregion

		#region Constructors
		private FunctionBodyEmitter(IR.Function function, MethodFactory methodFactory, FunctionLookup functionLookup)
		{
			Contract.Requires(function != null);
			Contract.Requires(methodFactory != null);
			Contract.Requires(functionLookup != null);

			Contract.Assert(function.Outputs.Length == 1);

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

			// Create the method and get its IL generator
			ILGenerator ilGenerator;
			method = methodFactory(function.Name, inputDescriptorsBuilder.Complete(), function.Outputs[0].StaticRepr.CliType, out ilGenerator);
			cil = new ILGeneratorMethodBodyWriter(ilGenerator);
			temporaryPool = new TemporaryLocalPool(cil, "$temp");

			foreach (var output in function.Outputs)
			{
				var localIndex = cil.DeclareLocal(output.StaticRepr.CliType, output.Name);
				locals.Add(output, VariableLocation.Local(localIndex));
			}
		}
		#endregion

		#region Methods
		public static MethodInfo Emit(IR.Function function, MethodFactory methodFactory, FunctionLookup functionLookup)
		{
			Contract.Requires(function != null);
			Contract.Requires(methodFactory != null);
			Contract.Requires(functionLookup != null);

			var emitter = new FunctionBodyEmitter(function, methodFactory, functionLookup);
			emitter.Emit();
			return emitter.method;
		}

		public void Emit()
		{
			returnTargetLabel = cil.CreateLabel("return");

			EmitStatements(function.Body);

			cil.MarkLabel(returnTargetLabel);
			EmitLoad(function.Outputs[0]);
			cil.Ret();
		}

		private void EmitStatements(ImmutableArray<Statement> statements)
		{
			foreach (var statement in statements)
				statement.Accept(this);
		}
		
		private void EmitLoad(Variable variable)
		{
			switch (variable.Kind)
			{
				case VariableKind.Input:
				case VariableKind.Local:
				case VariableKind.Output:
					cil.Load(GetLocalLocation(variable));
					break;

				default:
					throw new NotImplementedException();
			}
		}

		private EmitStoreScope BeginEmitStore(Variable variable)
		{
			switch (variable.Kind)
			{
				case VariableKind.Input:
				case VariableKind.Local:
				case VariableKind.Output:
					break;

				default:
					throw new NotImplementedException();
			}

			return new EmitStoreScope(this, variable);
		}

		private void EndEmitStore(Variable variable)
		{
			if (variable == null) return;

			switch (variable.Kind)
			{
				case VariableKind.Input:
				case VariableKind.Local:
				case VariableKind.Output:
					cil.Store(GetLocalLocation(variable));
					break;

				default:
					throw new NotImplementedException();
			}
		}

		private void EmitConversion(MRepr source, MRepr target)
		{
			if (source == target) return;

			if (target.IsAny && source.IsMValue) return;

			if (source.Type == target.Type)
			{
				if (source.StructuralClass == MStructuralClass.Scalar
					&& (target.StructuralClass == MStructuralClass.Array || target.IsAny))
				{
					// Boxing to array
					var boxMethod = typeof(MFullArray<>)
						.MakeGenericType(source.CliType)
						.GetMethod("CreateScalar");
					cil.Call(boxMethod);
					return;
				}

				// Upcast
				if (source.StructuralClass.IsArray && (target.IsArray || target.IsAny)) return;
			}

			throw new NotImplementedException(
				string.Format("Conversion from {0} to {1}.", source, target));
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
