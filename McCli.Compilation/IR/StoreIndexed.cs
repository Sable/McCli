using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compilation.IR
{
	public sealed class StoreIndexed : Expression
	{
		public readonly Variable Array;
		public readonly ImmutableArray<Variable> Indices;
		public readonly Variable Value;
		public readonly bool Cell;

		public StoreIndexed(Variable array, ImmutableArray<Variable> indices, Variable value, bool cell = false)
		{
			Contract.Requires(array != null);
			Contract.Requires(value != null);

			this.Array = array;
			this.Indices = indices;
			this.Value = value;
			this.Cell = cell;
		}

		public override void Accept(Visitor visitor)
		{
			visitor.VisitStoreIndexed(this);
		}
	}
}
