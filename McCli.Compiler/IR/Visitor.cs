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
		public virtual void VisitCopy(Copy node) { VisitExpression(node); }
		public virtual void VisitFor(RangeFor node) { VisitControlFlow(node); }
		public virtual void VisitFunction(Function node) { VisitNode(node); }
		public virtual void VisitIf(If node) { VisitControlFlow(node); }
		public virtual void VisitJump(Jump node) { VisitControlFlow(node); }
		public virtual void VisitLiteral(Literal node) { VisitExpression(node); }
		public virtual void VisitLoadParenthesized(LoadParenthesized node) { VisitExpression(node); }
		public virtual void VisitStaticCall(StaticCall node) { VisitExpression(node); }
		public virtual void VisitStoreParenthesized(StoreParenthesized node) { VisitExpression(node); }
		public virtual void VisitWhile(While node) { VisitControlFlow(node); }

		// Intermediate/categorization abstract base classes
		public virtual void VisitControlFlow(ControlFlow node) { VisitNode((Node)node); }
		public virtual void VisitExpression(Expression node) { VisitNode((Node)node); }

		// Root class
		public abstract void VisitNode(Node node);
	}
}
