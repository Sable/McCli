using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compilation.IR
{
	public sealed class ArrayStore : Assignment
	{
		public readonly Variable Array;
		public readonly Variable Index;
		public readonly Variable Value;

		public ArrayStore(Variable array, Variable index, Variable value)
		{
			Contract.Requires(array != null);
			Contract.Requires(index != null);
			Contract.Requires(value != null);

			this.Array = array;
			this.Index = index;
			this.Value = value;
		}

		public override void Accept(Visitor visitor)
		{
			visitor.VisitArrayStore(this);
		}
	}
}
