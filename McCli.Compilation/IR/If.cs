using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compilation.IR
{
	public sealed class If : Statement
	{
		public readonly Name Condition;
		public readonly ImmutableArray<Statement> Then;
		public readonly ImmutableArray<Statement> Else;

		public If(Name condition, ImmutableArray<Statement> @then, ImmutableArray<Statement> @else)
		{
			Contract.Requires(condition != null);

			this.Condition = condition;
			this.Then = @then;
			this.Else = @else;
		}
	}
}
