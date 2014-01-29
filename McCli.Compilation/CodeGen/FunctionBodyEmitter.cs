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

		#region LocalInfo
		struct LocalInfo
		{
			public LocalLocation Location;
			public Type Type;
		}
		#endregion

		#region Fields
		private readonly IR.Function function;
		private readonly MethodInfo method;
		private readonly ILGenerator ilGenerator;
		private readonly Dictionary<Variable, LocalInfo> locals = new Dictionary<Variable, LocalInfo>();
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
				parameterDescriptorsBuilder[i] = CreateParameter(function.Inputs[i], i);

			for (int i = 0; i < function.Outputs.Length; ++i)
			{
				int index = function.Inputs.Length + i;
				parameterDescriptorsBuilder[index] = CreateParameter(function.Outputs[i], index);
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
					ilGenerator.EmitLoad(GetLocalInfo(variable).Location);
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
					ilGenerator.EmitStore(GetLocalInfo(variable).Location);
					break;

				case VariableKind.Output:
					ilGenerator.EmitLoad(GetLocalInfo(variable).Location);
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

		private void EmitConvert(Type source, Type target)
		{
			if (source == target) return;

			if (source == typeof(double) && target == typeof(MArray<double>))
			{
				var createDoubleScalarMethod = typeof(MArray<double>).GetMethod("CreateScalar");
				ilGenerator.Emit(OpCodes.Call, createDoubleScalarMethod);
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		private LocalInfo GetLocalInfo(Variable variable)
		{
			LocalInfo info;
			if (!locals.TryGetValue(variable, out info))
			{
				var localBuilder = ilGenerator.DeclareLocal(typeof(MArray<double>));

				// Dynamic method don't support debug info
				if (!(method is DynamicMethod))
					localBuilder.SetLocalSymInfo(variable.Name);
				
				var location = LocalLocation.Variable(localBuilder.LocalIndex);
				info = new LocalInfo
				{
					Location = location,
					Type = GetDeclaredType(variable.StaticType)
				};
				locals.Add(variable, info);
			}

			return info;
		}

		private static Type GetDeclaredType(MType? staticType)
		{
			return staticType.HasValue ? staticType.Value.RuntimeType : typeof(MValue);
		}

		private ParameterDescriptor CreateParameter(Variable variable, int index)
		{
			Contract.Requires(variable != null);
			Contract.Requires(variable.Kind == VariableKind.Input || variable.Kind == VariableKind.Output);

			var type = GetDeclaredType(variable.StaticType);
			if (variable.Kind == VariableKind.Output) type = type.MakeByRefType();

			locals.Add(variable, new LocalInfo
			{
				Location = LocalLocation.Parameter(index),
				Type = type
			});

			var attributes = variable.Kind == VariableKind.Input ? ParameterAttributes.In : ParameterAttributes.Out; 
			return new ParameterDescriptor(type, attributes, variable.Name);
		}
		#endregion
	}
}
