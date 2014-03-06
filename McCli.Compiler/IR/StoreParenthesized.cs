using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compiler.IR
{
	/// <summary>
	/// An IR node for values stored into an array: <c>array(indices) = value</c>.
	/// </summary>
	public sealed class StoreParenthesized : Expression
	{
		#region Fields
		public readonly Variable Array;
		public readonly ImmutableArray<Variable> Indices;
		public readonly Variable Value;
		#endregion

		#region Constructors
		public StoreParenthesized(Variable array, ImmutableArray<Variable> indices, Variable value)
		{
			Contract.Requires(array != null);
			Contract.Requires(value != null);

			this.Array = array;
			this.Indices = indices;
			this.Value = value;
		}
		#endregion

		#region Properties
		public override int TargetCount
		{
			get { return 1; }
		}
		#endregion

		#region Methods
		public override Variable GetTarget(int index)
		{
			return Array;
		}

		public override void Accept(Visitor visitor)
		{
			visitor.VisitStoreParenthesized(this);
		}

		public override string ToDebugString()
		{
			return string.Format("{0}({1}) = {2}", Array.Name, CommaSeparate(Indices), Value.Name);
		}
		#endregion
	}
}
