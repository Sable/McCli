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
				MType sourceType;
				if (literal.Value is double)
				{
					ilGenerator.Emit(OpCodes.Ldc_R8, (double)literal.Value);
					sourceType = new MType(MPrimitiveClass.Double, MTypeForm.Scalar);
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

				// Clone if in boxed form
				var sourceType = (Type)copy.Value.StaticType;
				if (typeof(MValue).IsAssignableFrom(sourceType))
				{
					var deepCloneMethod = sourceType.GetMethod("DeepClone");
					ilGenerator.Emit(OpCodes.Call, deepCloneMethod);
				}

				EmitConversion(copy.Value.StaticType, copy.Target.StaticType);
			}
		}

		public override void VisitCallOrIndex(CallOrIndex callOrIndex)
		{
			base.VisitCallOrIndex(callOrIndex);
		}

		public override void VisitNode(IR.Node node)
		{
			throw new NotImplementedException();
		}
	}
}
