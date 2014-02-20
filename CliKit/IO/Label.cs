using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliKit.IO
{
	/// <summary>
	/// Represents a label used as branch targets when emitting CIL instructions.
	/// </summary>
	public struct Label : IEquatable<Label>
	{
		#region Fields
		internal readonly int index;
		#endregion

		#region Constructors
		internal Label(int index)
		{
			this.index = index;
		}
		#endregion

		#region Methods
		public bool Equals(Label other)
		{
			return index == other.index;
		}

		public override bool Equals(object obj)
		{
			return obj is Label && Equals((Label)obj);
		}

		public override int GetHashCode()
		{
			return index;
		}
		#endregion

		#region Operators
		public static bool operator ==(Label lhs, Label rhs) { return lhs.Equals(rhs); }
		public static bool operator !=(Label lhs, Label rhs) { return !lhs.Equals(rhs); }
		#endregion
	}
}
