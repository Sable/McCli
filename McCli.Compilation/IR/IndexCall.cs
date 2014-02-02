using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compilation.IR
{
	/// <summary>
	/// An expression in which either a variable is indexed.
	/// Represents both array accesses and function handle invocations (syntactic ambiguity).
	/// </summary>
	public sealed class IndexCall : Assignment
	{
		public readonly Variable Subject;
		public readonly ImmutableArray<Variable> Arguments;
		public readonly ImmutableArray<Variable> Targets;

		public IndexCall(Variable subject, ImmutableArray<Variable> arguments, ImmutableArray<Variable> targets)
		{
			this.Subject = subject;
			this.Arguments = arguments;
			this.Targets = targets;
		}

		public override void Accept(Visitor visitor)
		{
			visitor.VisitIndexCall(this);
		}
	}
}
