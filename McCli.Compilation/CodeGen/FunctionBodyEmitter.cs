using CliKit;
using McCli.Compilation.IR;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

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
		private readonly ILGenerator ilGenerator;
		private readonly Dictionary<Variable, LocalLocation> locals = new Dictionary<Variable, LocalLocation>();
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
				locals.Add(input, LocalLocation.Parameter(i));
				inputDescriptorsBuilder[i] = new ParameterDescriptor(input.StaticType.CliType, ParameterAttributes.In, input.Name);
			}

			// Create the method and get its IL generator
			method = methodFactory(function.Name, inputDescriptorsBuilder.Complete(), function.Outputs[0].StaticType.CliType, out ilGenerator);

			foreach (var output in function.Outputs)
			{
				var local = ilGenerator.DeclareLocal(output.StaticType.CliType);
				locals.Add(output, LocalLocation.Variable(local.LocalIndex));
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
			foreach (var statement in function.Body)
				statement.Accept(this);

			EmitLoad(function.Outputs[0]);
			ilGenerator.Emit(OpCodes.Ret);
		}
		
		private void EmitLoad(Variable variable)
		{
			switch (variable.Kind)
			{
				case VariableKind.Input:
				case VariableKind.Local:
				case VariableKind.Output:
					ilGenerator.EmitLoad(GetLocalLocation(variable));
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
					ilGenerator.EmitStore(GetLocalLocation(variable));
					break;

				default:
					throw new NotImplementedException();
			}
		}

		private void EmitConversion(MRepr source, MRepr target)
		{
			if (source == target) return;

			if (target.IsAny && source.IsMValue) return;

			if (source.Type == target.Type
				&& source.PrimitiveForm == MPrimitiveForm.Scalar
				&& target.PrimitiveForm == MPrimitiveForm.Array)
			{
				// Boxing to array
				var boxMethod = typeof(MDenseArray<>)
					.MakeGenericType(source.CliType)
					.GetMethod("CreateScalar");
				ilGenerator.Emit(OpCodes.Call, boxMethod);
				return;
			}

			throw new NotImplementedException();
		}

		private LocalLocation GetLocalLocation(Variable variable)
		{
			LocalLocation location;
			if (!locals.TryGetValue(variable, out location))
			{
				var localBuilder = ilGenerator.DeclareLocal(variable.StaticType.CliType);

				// Dynamic methods do not support debug info
				if (!(method is DynamicMethod))
					localBuilder.SetLocalSymInfo(variable.Name);
				
				location = LocalLocation.Variable(localBuilder.LocalIndex);
				locals.Add(variable, location);
			}

			return location;
		}
		#endregion
	}
}
