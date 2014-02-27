using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compiler.IR
{
	/// <summary>
	/// A call to a "statically" resolved function (not through a function handle).
	/// </summary>
	public sealed class StaticCall : Expression
	{
		public readonly ImmutableArray<Variable> Targets;
		public readonly string FunctionName;
		public readonly ImmutableArray<Variable> Arguments;

		public StaticCall(ImmutableArray<Variable> targets, string functionName, ImmutableArray<Variable> arguments)
		{
			Contract.Requires(functionName != null);

			this.Targets = targets;
			this.FunctionName = functionName;
			this.Arguments = arguments;
		}

		public override void Accept(Visitor visitor)
		{
			visitor.VisitStaticCall(this);
		}

		public override string ToDebugString()
		{
			string format = string.Empty;
			if (Targets.Length == 1) format += "{0} = ";
			else if (Targets.Length >= 2) format += "[{0}] = ";
			format += "{1}({2})";
			return string.Format(format, CommaSeparate(Targets), FunctionName, CommaSeparate(Arguments));
		}
	}
}
