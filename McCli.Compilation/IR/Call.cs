using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compilation.IR
{
	public sealed class Call : Statement
	{
		public readonly Name Function;
		public readonly ImmutableArray<Name> Arguments;
		public readonly ImmutableArray<Name> Targets;

		public Call(Name function, ImmutableArray<Name> arguments, ImmutableArray<Name> targets)
		{
			this.Function = function;
			this.Arguments = arguments;
			this.Targets = targets;
		}
	}
}
