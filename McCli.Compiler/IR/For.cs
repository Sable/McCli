using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compiler.IR
{
	/// <summary>
	/// Represents the MatLab for loop construct.
	/// </summary>
	public sealed class For : ControlFlow
	{
		public readonly Variable Iterator;
		public readonly Variable Collection;
		public readonly ImmutableArray<Statement> Body;

		public For(Variable iterator, Variable collection, ImmutableArray<Statement> body)
		{
			Contract.Requires(iterator != null);
			Contract.Requires(collection != null);

			this.Iterator = iterator;
			this.Collection = collection;
			this.Body = body;
		}

		public For(Variable iterator, Variable collection, params Statement[] body)
			: this(iterator, collection, ImmutableArray.Create(body)) { }

		public override void Accept(Visitor visitor)
		{
			visitor.VisitFor(this);
		}
	}
}
