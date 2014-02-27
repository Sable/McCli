using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compiler.IR
{
	/// <summary>
	/// An expression in which an array's element is loaded, or a function handle is called: <c>[targets] = subject(arguments)</c>.
	/// </summary>
	public sealed class LoadParenthesized : Expression
	{
		public readonly ImmutableArray<Variable> Targets;
		public readonly Variable Subject;
		public readonly ImmutableArray<Variable> Arguments;

		public LoadParenthesized(ImmutableArray<Variable> targets, Variable subject, ImmutableArray<Variable> arguments)
		{
			Contract.Requires(subject != null);

			this.Targets = targets;
			this.Subject = subject;
			this.Arguments = arguments;
		}

		public override void Accept(Visitor visitor)
		{
			visitor.VisitLoadParenthesized(this);
		}

		public override string ToDebugString()
		{
			string format = string.Empty;
			if (Targets.Length == 1) format += "{0} = ";
			else if (Targets.Length >= 2) format += "[{0}] = ";
			format += "{1}({2})";
			return string.Format(format, CommaSeparate(Targets), Subject.Name, CommaSeparate(Arguments));
		}
	}
}
