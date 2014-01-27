using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compilation.IR
{
	public sealed class Name
	{
		public readonly string Value;

		public Name(string value)
		{
			Contract.Requires(value != null);

			this.Value = value;
		}

		public static implicit operator string(Name name)
		{
			return name == null ? null : name.Value;
		}
	}
}
