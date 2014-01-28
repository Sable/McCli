using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compilation.IR
{
	/// <summary>
	/// An expression in which either a function is called or
	/// an array is indexed (syntactically ambiguous).
	/// </summary>
	public sealed class CallOrIndex : Assignment
	{
		public readonly Variable Function;
		public readonly ImmutableArray<Variable> Arguments;
		public readonly ImmutableArray<Variable> Targets;

		public CallOrIndex(Variable function, ImmutableArray<Variable> arguments, ImmutableArray<Variable> targets)
		{
			this.Function = function;
			this.Arguments = arguments;
			this.Targets = targets;
		}

		public override void Accept(Visitor visitor)
		{
			visitor.VisitCallOrIndex(this);
		}
	}
}
