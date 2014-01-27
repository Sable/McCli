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
		public readonly Name Function;
		public readonly ImmutableArray<Name> Arguments;
		public readonly ImmutableArray<Name> Targets;

		public CallOrIndex(Name function, ImmutableArray<Name> arguments, ImmutableArray<Name> targets)
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
