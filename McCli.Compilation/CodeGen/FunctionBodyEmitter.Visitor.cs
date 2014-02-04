using McCli.Compilation.IR;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compilation.CodeGen
{
	partial class FunctionBodyEmitter : Visitor
	{
		public override void VisitLiteral(Literal literal)
		{
			using (BeginEmitStore(literal.Target))
			{
				MRepr sourceType;
				if (literal.Value is double)
				{
					ilGenerator.Emit(OpCodes.Ldc_R8, (double)literal.Value);
					sourceType = MPrimitiveClass.Double.AsScalarType();
				}
				else
				{
					throw new NotImplementedException();
				}

				EmitConversion(sourceType, literal.Target.StaticType);
			}
		}

		public override void VisitCopy(Copy copy)
		{
			Contract.Requires(copy.Value.StaticType == copy.Target.StaticType);

			using (BeginEmitStore(copy.Target))
			{
				EmitLoad(copy.Value);
				EmitCloneIfBoxed(copy.Value.StaticType);

				EmitConversion(copy.Value.StaticType, copy.Target.StaticType);
			}
		}

		public override void VisitStaticCall(StaticCall staticCall)
		{
			Contract.Assert(staticCall.Targets.Length == 1);
			using (BeginEmitStore(staticCall.Targets[0]))
			{
				var argumentTypes = staticCall.Arguments.Select(a => a.StaticType);
				var function = functionLookup(staticCall.Name, argumentTypes);

				for (int i = 0; i < staticCall.Arguments.Length; ++i)
				{
					var argument = staticCall.Arguments[i];
					EmitLoad(argument);
					EmitConversion(argument.StaticType, function.InputTypes[i]);
				}

				ilGenerator.Emit(OpCodes.Call, function.Method);
				EmitConversion(function.OutputType, staticCall.Targets[0].StaticType);
			}
		}

		public override void VisitLoadCall(LoadCall loadCall)
		{
			Contract.Assert(loadCall.Targets.Length == 1);
			Contract.Assert(loadCall.Subject.StaticType.Class is MPrimitiveClass);

			using (BeginEmitStore(loadCall.Targets[0]))
			{
				EmitLoad(loadCall.Subject);

				if (loadCall.Arguments.Length == 0)
				{
					EmitCloneIfBoxed(loadCall.Subject.StaticType);
				}
				else
				{
					throw new NotImplementedException();
				}
			}
		}

		public override void VisitNode(IR.Node node)
		{
			throw new NotImplementedException();
		}


		private void EmitCloneIfBoxed(MRepr type)
		{
			var sourceType = type.CliType;
			if (typeof(MValue).IsAssignableFrom(sourceType))
			{
				var deepCloneMethod = sourceType.GetMethod("DeepClone");
				ilGenerator.Emit(OpCodes.Callvirt, deepCloneMethod);
				if (!sourceType.IsAssignableFrom(deepCloneMethod.ReturnType))
					ilGenerator.Emit(OpCodes.Castclass, sourceType);
			}
		}
	}
}
