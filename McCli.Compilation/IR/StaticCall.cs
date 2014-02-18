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
		public readonly ImmutableArray<Variable> Targets;
		public readonly string Name;
		public readonly ImmutableArray<Variable> Arguments;

		public StaticCall(ImmutableArray<Variable> targets, string name, ImmutableArray<Variable> arguments)
		{
			Contract.Requires(name != null);

			this.Targets = targets;
			this.Name = name;
			this.Arguments = arguments;
		}

		public override void Accept(Visitor visitor)
		{
			visitor.VisitStaticCall(this);
		}
	}
}
