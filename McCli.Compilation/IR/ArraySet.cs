using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compilation.IR
{
	public sealed class ArraySet : Statement
	{
		public readonly Name Array;
		public readonly Name Index;
		public readonly Name Value;

		public ArraySet(Name array, Name index, Name value)
		{
			Contract.Requires(array != null);
			Contract.Requires(index != null);
			Contract.Requires(value != null);

			this.Array = array;
			this.Index = index;
			this.Value = value;
		}
	}
}
