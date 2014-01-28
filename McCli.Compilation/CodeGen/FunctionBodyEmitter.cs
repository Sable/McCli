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
	public sealed class FunctionBodyEmitter : IR.Visitor
	{
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

			var parameters = function.Inputs
				.Select(var => new ParameterDescriptor(var.StaticType.BoxedType, var.Name))
				.Concat(function.Outputs
					.Select(var => new ParameterDescriptor(var.StaticType.BoxedType.MakeByRefType(), ParameterAttributes.In | ParameterAttributes.Out, var.Name))
				).ToImmutableArray();
			method = methodFactory(function.Name, parameters, ParameterDescriptor.VoidReturn, out ilGenerator);
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

		public override void VisitLiteral(Literal literal)
		{
			if (literal.Value is double)
			{
				ilGenerator.Emit(OpCodes.Ldc_R8, (double)literal.Value);
				var createDoubleScalarMethod = typeof(MArray<double>).GetMethod("CreateScalar");
				ilGenerator.Emit(OpCodes.Call, createDoubleScalarMethod);
			}
			else
			{
				throw new NotImplementedException();
			}

			EmitStore(literal.Target);
		}

		public override void VisitCopy(Copy copy)
		{
			Contract.Requires(copy.Value.StaticType == copy.Target.StaticType);

			if (copy.Value.StaticType == MNumericClass.Double)
			{
				EmitLoad(copy.Value);
				var cloneDoubleArrayMethod = typeof(MArray<double>).GetMethod("DeepClone");
				ilGenerator.Emit(OpCodes.Call, cloneDoubleArrayMethod);
			}
			else
			{
				throw new NotImplementedException();
			}

			EmitStore(copy.Target);
		}

		public override void VisitNode(IR.Node node)
		{
			throw new NotImplementedException();
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

		private void EmitStore(Variable variable)
		{
			switch (variable.Kind)
			{
				case VariableKind.Input:
				case VariableKind.Local:
					ilGenerator.EmitStore(GetLocalLocation(variable));
					break;

				case VariableKind.Output:
					ilGenerator.EmitLoad(GetLocalLocation(variable));
					ilGenerator.Emit(OpCodes.Stind_Ref);
					break;

				default:
					throw new NotImplementedException();
			}
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
