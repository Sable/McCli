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
			}
		}

		public override void VisitCopy(Copy copy)
		{
			Contract.Requires(copy.Value.StaticType == copy.Target.StaticType);

			using (BeginEmitStore(copy.Target))
			{
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
			}
		}

		public override void VisitNode(IR.Node node)
		{
			throw new NotImplementedException();
		}
	}
}
