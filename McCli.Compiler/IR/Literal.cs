using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compiler.IR
{
	public sealed class Literal : Expression
	{
		public readonly Variable Target;
		public readonly object Value;

		public Literal(Variable target, double value)
		{
			Contract.Requires(target != null);

			this.Target = target;
			this.Value = value;
		}

		public Literal(Variable target, char character)
		{
			Contract.Requires(target != null);

			this.Target = target;
			this.Value = character;
		}

		public override void Accept(Visitor visitor)
		{
			visitor.VisitLiteral(this);
		}
	}
}
