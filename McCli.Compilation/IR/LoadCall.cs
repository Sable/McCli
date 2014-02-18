using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compilation.IR
{
	/// <summary>
	/// An expression in which an array's element is loaded, or a function handle is called.
	/// </summary>
	public sealed class LoadCall : Expression
	{
		public readonly ImmutableArray<Variable> Targets;
		public readonly Variable Subject;
		public readonly ImmutableArray<Variable> Arguments;
		public readonly bool Cell;

		public LoadCall(ImmutableArray<Variable> targets, Variable subject, ImmutableArray<Variable> arguments, bool cell = false)
		{
			this.Targets = targets;
			this.Subject = subject;
			this.Arguments = arguments;
			this.Cell = cell;
		}

		public override void Accept(Visitor visitor)
		{
			visitor.VisitLoadCall(this);
		}
	}
}
