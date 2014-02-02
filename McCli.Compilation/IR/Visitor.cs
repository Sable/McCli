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
		public virtual void VisitArrayStore(StoreIndexed arrayStore) { VisitAssignment((Expression)arrayStore); }
		public virtual void VisitLoadCall(LoadCall loadCall) { VisitAssignment((Expression)loadCall); }
		public virtual void VisitStoreIndexed(StoreIndexed storeIndexed) { VisitAssignment((Expression)storeIndexed); }
		public virtual void VisitStaticCall(StaticCall staticCall) { VisitAssignment((Expression)staticCall); }
		public virtual void VisitCopy(Copy copy) { VisitAssignment((Expression)copy); }
		public virtual void VisitFunction(Function function) { VisitNode((Node)function); }
		public virtual void VisitIf(If @if) { VisitNode((ControlFlow)@if); }
		public virtual void VisitLiteral(Literal literal) { VisitAssignment((Expression)literal); }

		// Intermediate/categorization abstract base classes
		public virtual void VisitAssignment(Expression assignment) { VisitNode((Node)assignment); }
		public virtual void VisitControlFlow(ControlFlow controlFlow) { VisitNode((Node)controlFlow); }

		// Root class
		public abstract void VisitNode(Node node);
	}
}
