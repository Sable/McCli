using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compiler.IR
{
	public sealed class If : ControlFlow
	{
		public readonly Variable Condition;
		public readonly ImmutableArray<Statement> Then;
		public readonly ImmutableArray<Statement> Else;

		public If(Variable condition, ImmutableArray<Statement> @then, ImmutableArray<Statement> @else)
		{
			Contract.Requires(condition != null);

			this.Condition = condition;
			this.Then = @then;
			this.Else = @else;
		}

		public bool HasElse
		{
			get { return Else.Length > 0; }
		}

		public override void Accept(Visitor visitor)
		{
			visitor.VisitIf(this);
		}

		public override string ToDebugString()
		{
			string format = "if ({0}) [{1} statements]";
			if (HasElse) format += " else [{2} statements]";
			return string.Format(format, Condition.Name, Then.Length, Then.Length);
		}
	}
}
