using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compilation.IR
{
	/// <summary>
	/// A call to a "statically" resolved function (not through a function handle).
	/// </summary>
	public sealed class StaticCall : Expression
	{
		public readonly string Name;
		public readonly ImmutableArray<Variable> Arguments;
		public readonly ImmutableArray<Variable> Targets;

		public StaticCall(string name, ImmutableArray<Variable> arguments, ImmutableArray<Variable> targets)
		{
			Contract.Requires(name != null);

			this.Name = name;
			this.Arguments = arguments;
			this.Targets = targets;
		}

		public override void Accept(Visitor visitor)
		{
			visitor.VisitStaticCall(this);
		}
	}
}
