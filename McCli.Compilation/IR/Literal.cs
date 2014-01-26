using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compilation.IR
{
	public sealed class Literal : Statement
	{
		public readonly Name Target;
		public readonly object Value;

		public Literal(Name target, object value)
		{
			Contract.Requires(target != null);
			Contract.Requires(value != null);

			this.Target = target;
			this.Value = value;
		}
	}
}
