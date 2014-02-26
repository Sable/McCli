using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace McCli.Builtins
{
	/// <summary>
	/// Implements MatLab builtins for array manipulation.
	/// http://www.mathworks.com/help/matlab/array-manipulation.html
	/// </summary>
	[MatlabLibrary]
	public static class ArrayManipulation
	{
		#region Transpose
		public static MArray<TScalar> transpose<[AnyPrimitive] TScalar>(MArray<TScalar> array)
		{
			Contract.Requires(array != null);
			if (array.IsHigherDimensional) throw new MArrayShapeException();

			var shape = array.Shape;
			var result = new MFullArray<TScalar>(shape.ColumnCount, shape.RowCount);
			for (int row = 0; row < shape.RowCount; ++row)
				for (int column = 0; column < shape.ColumnCount; ++column)
					result[row * shape.ColumnCount + column] = array[column * shape.RowCount + row];
			return result;
		}

		internal static TScalar transpose<[AnyPrimitive] TScalar>(TScalar value)
		{
			return value;
		}
		#endregion

		// TODO: diag, sort, repmat, etc
	}
}
