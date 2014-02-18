using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compilation.IR
{
	/// <summary>
	/// IR node where the value of a variable is assigned to another.
	/// </summary>
	public sealed class Copy : Expression
	{
		public readonly Variable Target, Value;

		public Copy(Variable target, Variable value)
		{
			Contract.Requires(target != null);
			Contract.Requires(value != null);

			this.Target = target;
			this.Value = value;
		}

		public override void Accept(Visitor visitor)
		{
			visitor.VisitCopy(this);
		}
	}
}
