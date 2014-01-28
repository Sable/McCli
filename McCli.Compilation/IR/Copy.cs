using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compilation.IR
{
	public sealed class Copy : Assignment
	{
		public readonly Variable Value, Target;

		public Copy(Variable value, Variable target)
		{
			Contract.Requires(value != null);
			Contract.Requires(target != null);

			this.Value = value;
			this.Target = target;
		}

		public override void Accept(Visitor visitor)
		{
			visitor.VisitCopy(this);
		}
	}
}
