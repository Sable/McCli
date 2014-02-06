using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compilation.IR
{
	/// <summary>
	/// Abstract base class for IR node visitors.
	/// </summary>
	public abstract class Visitor
	{
		// Concrete classes
		public virtual void VisitStoreIndexed(StoreIndexed storeIndexed) { VisitExpression((Expression)storeIndexed); }
		public virtual void VisitLoadCall(LoadCall loadCall) { VisitExpression((Expression)loadCall); }
		public virtual void VisitStaticCall(StaticCall staticCall) { VisitExpression((Expression)staticCall); }
		public virtual void VisitCopy(Copy copy) { VisitExpression((Expression)copy); }
		public virtual void VisitFunction(Function function) { VisitNode((Node)function); }
		public virtual void VisitIf(If @if) { VisitNode((ControlFlow)@if); }
		public virtual void VisitLiteral(Literal literal) { VisitExpression((Expression)literal); }

		// Intermediate/categorization abstract base classes
		public virtual void VisitExpression(Expression assignment) { VisitNode((Node)assignment); }
		public virtual void VisitControlFlow(ControlFlow controlFlow) { VisitNode((Node)controlFlow); }

		// Root class
		public abstract void VisitNode(Node node);
	}
}
