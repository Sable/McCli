﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
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

		public override string ToDebugString()
		{
			return string.Format(CultureInfo.InvariantCulture, 
				"{0} = " + (Value is string || Value is char ? "'{1}'" : "{1}"), Target.Name, Value);
		}
	}
}
