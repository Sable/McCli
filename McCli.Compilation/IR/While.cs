using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compilation.IR
{
	/// <summary>
	/// Represents a MatLab while loop.
	/// </summary>
	public sealed class While : ControlFlow
	{
		public readonly Variable Condition;
		public readonly ImmutableArray<Statement> Body;

		public While(Variable condition, ImmutableArray<Statement> body)
		{
			Contract.Requires(condition != null);
			this.Condition = condition;
			this.Body = body;
		}

		public override void Accept(Visitor visitor)
		{
			visitor.VisitWhile(this);
		}
	}
}
