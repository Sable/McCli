using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compiler.IR
{
	/// <summary>
	/// Abstract base class for IR node visitors.
	/// </summary>
	public abstract class Visitor
	{
		// Concrete classes
		public virtual void VisitCopy(Copy node) { VisitExpression((Expression)node); }
		public virtual void VisitFor(RangeFor node) { VisitNode((ControlFlow)node); }
		public virtual void VisitFunction(Function node) { VisitNode((Node)node); }
		public virtual void VisitIf(If node) { VisitNode((ControlFlow)node); }
		public virtual void VisitJump(Jump node) { VisitNode((ControlFlow)node); }
		public virtual void VisitLiteral(Literal node) { VisitExpression((Expression)node); }
		public virtual void VisitLoadParenthesized(LoadParenthesized node) { VisitExpression((Expression)node); }
		public virtual void VisitStaticCall(StaticCall node) { VisitExpression((Expression)node); }
		public virtual void VisitStoreParenthesized(StoreParenthesized node) { VisitExpression((Expression)node); }
		public virtual void VisitWhile(While node) { VisitNode((ControlFlow)node); }

		// Intermediate/categorization abstract base classes
		public virtual void VisitControlFlow(ControlFlow node) { VisitNode((Node)node); }
		public virtual void VisitExpression(Expression node) { VisitNode((Node)node); }

		// Root class
		public abstract void VisitNode(Node node);
	}
}
