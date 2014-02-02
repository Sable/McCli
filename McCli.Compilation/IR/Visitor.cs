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
		public virtual void VisitArrayStore(ArrayStore arrayStore) { VisitAssignment((Assignment)arrayStore); }
		public virtual void VisitIndexCall(IndexCall indexCall) { VisitAssignment((Assignment)indexCall); }
		public virtual void VisitStaticCall(StaticCall staticCall) { VisitAssignment((Assignment)staticCall); }
		public virtual void VisitCopy(Copy copy) { VisitAssignment((Assignment)copy); }
		public virtual void VisitFunction(Function function) { VisitNode((Node)function); }
		public virtual void VisitIf(If @if) { VisitNode((ControlFlow)@if); }
		public virtual void VisitLiteral(Literal literal) { VisitAssignment((Assignment)literal); }

		// Intermediate/categorization abstract base classes
		public virtual void VisitAssignment(Assignment assignment) { VisitNode((Node)assignment); }
		public virtual void VisitControlFlow(ControlFlow controlFlow) { VisitNode((Node)controlFlow); }

		// Root class
		public abstract void VisitNode(Node node);
	}
}
