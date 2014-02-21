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
		public readonly Variable LowerBound;
		public readonly Variable Increment;
		public readonly Variable UpperBound;
		public readonly ImmutableArray<Statement> Body;

		public RangeFor(Variable iterator, Variable lowerBound, Variable increment, Variable upperBound, ImmutableArray<Statement> body)
		{
			Contract.Requires(iterator != null);
			Contract.Requires(lowerBound != null);
			Contract.Requires(upperBound != null);

			this.Iterator = iterator;
			this.LowerBound = lowerBound;
			this.Increment = increment;
			this.UpperBound = upperBound;
			this.Body = body;
		}

		public RangeFor(Variable iterator, Variable lowerBound, Variable increment, Variable upperBound, params Statement[] body)
			: this(iterator, lowerBound, increment, upperBound, ImmutableArray.Create(body)) { }

		public override void Accept(Visitor visitor)
		{
			visitor.VisitFor(this);
		}
	}
}
