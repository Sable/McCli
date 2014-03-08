using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace McCli
{
	/// <summary>
	/// Provides utility functions for dealing with MatLab strings (char arrays).
	/// </summary>
	public static class MStrings
	{
		public static string ToString(MArray<char> array)
		{
			Contract.Requires(array != null);

			var shape = array.Shape;
			if (!shape.IsRowVector) throw new MArrayShapeException();

			// TODO: Don't assume a full array
			return new string(((MFullArray<char>)array).BackingArray, 0, shape.ColumnCount);
		}

		public static MFullArray<char> FromString(string str)
		{
			Contract.Requires(str != null);
			return MFullArray<char>.CreateRowVector(str.ToCharArray());
		}
	}
}
