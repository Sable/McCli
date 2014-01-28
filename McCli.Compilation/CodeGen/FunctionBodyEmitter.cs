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
			private readonly object token;

			public EmitStoreScope(FunctionBodyEmitter instance, object token)
			{
				this.instance = instance;
				this.token = token;
			}

			public void Dispose()
			{
				instance.EndEmitStore(token);
			}
		}
		#endregion

		#region Fields
		private readonly IR.Function function;
		private readonly MethodInfo method;
		private readonly ILGenerator ilGenerator;
		private readonly Dictionary<Variable, LocalLocation> locals = new Dictionary<Variable, LocalLocation>();
		#endregion

		#region Constructors
		private FunctionBodyEmitter(IR.Function function, MethodFactory methodFactory)
		{
			Contract.Requires(function != null);
			Contract.Requires(methodFactory != null);

			this.function = function;

			// Handle method parameters
			var parameterDescriptorsBuilder = new ImmutableArray<ParameterDescriptor>.Builder(
				function.Inputs.Length + function.Outputs.Length);

			for (int i = 0; i < function.Inputs.Length; ++i)
			{
				var input = function.Inputs[i];
				parameterDescriptorsBuilder[i] = new ParameterDescriptor(input.StaticType.BoxedType, input.Name);
				locals.Add(input, LocalLocation.Parameter(i));
			}

			for (int i = 0; i < function.Outputs.Length; ++i)
			{
				var output = function.Outputs[i];
				int parameterIndex = function.Inputs.Length + i;
				parameterDescriptorsBuilder[parameterIndex] = new ParameterDescriptor(
					output.StaticType.BoxedType.MakeByRefType(),
					ParameterAttributes.In | ParameterAttributes.Out, output.Name);
				locals.Add(output, LocalLocation.Parameter(parameterIndex));
			}

			// Create the method and get its IL generator
			method = methodFactory(function.Name, parameterDescriptorsBuilder.Complete(),
				ParameterDescriptor.VoidReturn, out ilGenerator);
		}
		#endregion

		#region Methods
		public static MethodInfo Emit(IR.Function function, MethodFactory methodFactory)
		{
			Contract.Requires(function != null);
			Contract.Requires(methodFactory != null);

			var emitter = new FunctionBodyEmitter(function, methodFactory);
			emitter.Emit();
			return emitter.method;
		}

		public void Emit()
		{
			foreach (var statement in function.Body)
				statement.Accept(this);
			ilGenerator.Emit(OpCodes.Ret);
		}
		
		private void EmitLoad(Variable variable)
		{
			switch (variable.Kind)
			{
				case VariableKind.Input:
				case VariableKind.Local:
					ilGenerator.EmitLoad(GetLocalLocation(variable));
					break;

				default:
					throw new NotImplementedException();
			}
		}

		private EmitStoreScope BeginEmitStore(Variable variable)
		{
			object token = null;
			switch (variable.Kind)
			{
				case VariableKind.Input:
				case VariableKind.Local:
					ilGenerator.EmitStore(GetLocalLocation(variable));
					break;

				case VariableKind.Output:
					ilGenerator.EmitLoad(GetLocalLocation(variable));
					token = variable;
					break;

				default:
					throw new NotImplementedException();
			}

			return new EmitStoreScope(this, token);
		}

		private void EndEmitStore(object token)
		{
			if (token == null) return;

			Contract.Assert(token is Variable);
			ilGenerator.Emit(OpCodes.Stind_Ref);
		}

		private LocalLocation GetLocalLocation(Variable variable)
		{
			LocalLocation location;
			if (!locals.TryGetValue(variable, out location))
			{
				var localBuilder = ilGenerator.DeclareLocal(typeof(MArray<double>));

				// Dynamic method don't support debug info
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
