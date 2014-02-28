using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliKit
{
	/// <summary>
	/// Abstract base class for opcode visitors.
	/// </summary>
	/// <typeparam name="T">The type of the visit argument.</typeparam>
	public abstract class OpcodeVisitor<T>
	{
		public virtual void VisitArithmetic(ArithmeticOpcode opcode, T param) { VisitFallback(opcode, param); }
		public virtual void VisitBranch(BranchOpcode opcode, T param) { VisitFallback(opcode, param); }
		public virtual void VisitCall(CallOpcode opcode, T param) { VisitFallback(opcode, param); }
		public virtual void VisitCast(CastOpcode opcode, T param) { VisitFallback(opcode, param); }
		public virtual void VisitComparison(ComparisonOpcode opcode, T param) { VisitFallback(opcode, param); }
		public virtual void VisitConversion(ConversionOpcode opcode, T param) { VisitFallback(opcode, param); }
		public virtual void VisitElementReference(ElementReferenceOpcode opcode, T param) { VisitFallback(opcode, param); }
		public virtual void VisitFieldReference(FieldReferenceOpcode opcode, T param) { VisitFallback(opcode, param); }
		public virtual void VisitIndirectReference(IndirectReferenceOpcode opcode, T param) { VisitFallback(opcode, param); }
		public virtual void VisitLoadConstant(LoadConstantOpcode opcode, T param) { VisitFallback(opcode, param); }
		public virtual void VisitVariableReference(VariableReferenceOpcode opcode, T param) { VisitFallback(opcode, param); }

		public virtual void VisitBreak(T param) { VisitFallback(Opcode.Break, param); }
		public virtual void VisitNop(T param) { VisitFallback(Opcode.Nop, param); }
		public virtual void VisitReturn(T param) { VisitFallback(Opcode.Ret, param); }
		public virtual void VisitSizeof(T param) { VisitFallback(Opcode.Sizeof, param); }
		public virtual void VisitPopOrDup(Opcode opcode, T param) { VisitFallback(opcode, param); }
		public virtual void VisitThrowOrRethrow(Opcode opcode, T param) { VisitFallback(opcode, param); }
		public virtual void VisitOther(Opcode opcode, T param) { VisitFallback(opcode, param); }

		public abstract void VisitFallback(Opcode opcode, T param);
	}
}
