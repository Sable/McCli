using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compilation.IR
{
	/// <summary>
	/// A statement in which the value at an array's index is obtained.
	/// <c>target = array(index)</c>
	/// </summary>
	public sealed class ArrayGet : Statement
	{
		public readonly Name Array;
		public readonly Name Index;
		public readonly Name Target;

		public ArrayGet(Name array, Name index, Name target)
		{
			Contract.Requires(array != null);
			Contract.Requires(index != null);
			Contract.Requires(target != null);

			this.Array = array;
			this.Index = index;
			this.Target = target;
		}
	}
}
