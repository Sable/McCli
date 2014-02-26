using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compiler.IR
{
	/// <summary>
	/// Represents the MatLab for loop construct when used with a range: <c>for i = a:b</c>.
	/// Unfortunately, tamer does not support the more general <c>for i = array</c>.
	/// </summary>
	public sealed class RangeFor : ControlFlow
	{
		public readonly Variable Iterator;
		public readonly Variable From;
		public readonly Variable Step;
		public readonly Variable To;
		public readonly ImmutableArray<Statement> Body;

		public RangeFor(Variable iterator, Variable from, Variable step, Variable to, ImmutableArray<Statement> body)
		{
			Contract.Requires(iterator != null);
			Contract.Requires(from != null);
			Contract.Requires(to != null);

			this.Iterator = iterator;
			this.From = from;
			this.Step = step;
			this.To = to;
			this.Body = body;
		}

		public RangeFor(Variable iterator, Variable from, Variable step, Variable to, params Statement[] body)
			: this(iterator, from, step, to, ImmutableArray.Create(body)) { }

		public override void Accept(Visitor visitor)
		{
			visitor.VisitFor(this);
		}
	}
}
