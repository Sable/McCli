using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// Miscellaneous functions that complement MatLab builtins.
	/// </summary>
	public static class Utilities
	{
		public static bool IsIntegralFloat(double value)
		{
			return Math.Floor(value) == value;
		}

		public static int ToInt(double value)
		{
			var i = (int)value;
			if (i != value) throw new InvalidCastException();
			return i;
		}

		public static int ToIntScalar(MArray<double> value)
		{
			Contract.Requires(value != null);
			if (!value.IsScalar) throw new MArrayShapeException();
			return ToInt(value[0]);
		}
		
		public static MArrayShape ToShape(double rowCount, double columnCount)
		{
			int rowCountInt = ToInt(rowCount);
			int columnCountInt = ToInt(columnCount);

			// Ensure neither are negative
			if ((rowCountInt | columnCountInt) < 0) throw new MArrayShapeException();
			
			return new MArrayShape(rowCountInt, columnCountInt);
		}

		public static bool IsTrue(MValue value)
		{
			// "An evaluated expression is true when the result is nonempty
			// and contains all nonzero elements (logical or real numeric).
			// Otherwise, the expression is false."
			Contract.Requires(value != null);

			var type = value.Repr;
			if (type.IsComplex) return false;

			if (type.IsArray)
			{
				int count = value.Count;
				if (count == 0) return false;
				
				var doubleArray = value as MFullArray<double>;
				if (doubleArray != null)
				{
					for (int i = 0; i < count; ++i)
						if (doubleArray[i] == 0)
							return false;
					return true;
				}

				var logicalArray = value as MFullArray<bool>;
				if (logicalArray != null)
				{
					for (int i = 0; i < count; ++i)
						if (!logicalArray[i])
							return false;
					return true;
				}
			}

			throw new NotImplementedException();
		}

		public static string AsString(MFullArray<char> array)
		{
			Contract.Requires(array != null);

			var shape = array.Shape;
			if (!shape.IsRowVector) throw new MArrayShapeException();

			return new string(array.BackingArray, 0, shape.ColumnCount);
		}

		#region For Loops
		public static int GetForSliceCount(MValue value)
		{
			Contract.Requires(value != null);
			var shape = value.Shape;
			var count = shape.Count;
			return count == 0 ? 0 : (count / shape.RowCount);
		}

		public static MArray<TScalar> GetForSlice<[AnyArrayable] TScalar>(MArray<TScalar> array, int index)
		{
			Contract.Requires(array != null);

			var arrayShape = array.Shape;
			Contract.Assert(!arrayShape.IsHigherDimensional);

			int baseArrayIndex = arrayShape.RowCount * index;
			var slice = new MFullArray<TScalar>(new MArrayShape(arrayShape.RowCount, 1));
			for (int i = 0; i < arrayShape.RowCount; ++i)
				slice[i] = array[baseArrayIndex + i];
			return slice;
		}
		#endregion

		#region Subsref/Subsasgn
		public static MArray<TScalar> Subsref<TScalar>(MArray<TScalar> array, MArray<double> indices)
		{
			Contract.Requires(array != null);
			Contract.Requires(indices != null);

			var indicesShape = indices.Shape;
			if (indicesShape.IsEmpty) return new MFullArray<TScalar>(MArrayShape.Empty);
			Contract.Assert(indicesShape.ColumnCount == 1);

			var result = new MFullArray<TScalar>(new MArrayShape(indicesShape.RowCount, 1));
			for (int i = 0; i < indicesShape.RowCount; ++i)
				result[i] = Subsref(array, ToInt(indices[i]));
			return result;
		}

		public static TScalar Subsref<TScalar>(MArray<TScalar> array, double rowIndex, double columnIndex)
		{
			return Subsref(array, ToInt(rowIndex), ToInt(columnIndex));
		}

		private static TScalar Subsref<TScalar>(MArray<TScalar> array, int rowIndex, int columnIndex)
		{
			Contract.Requires(array != null);
			Contract.Requires(!array.IsHigherDimensional);

			if (rowIndex < 1 || rowIndex > array.RowCount
				|| columnIndex < 1 || columnIndex > array.ColumnCount)
				throw new ArgumentOutOfRangeException();

			int index = rowIndex + (columnIndex - 1) * array.RowCount;
			return Subsref(array, index);
		}

		private static TScalar Subsref<TScalar>(MArray<TScalar> array, double index)
		{
			return Subsref(array, Utilities.ToInt(index));
		}

		private static TScalar Subsref<TScalar>(MArray<TScalar> array, int index)
		{
			return array[index - 1];
		}

		public static MArray<TScalar> Subsref<TScalar>(MArray<TScalar> array, MArray<bool> mask)
		{
			Contract.Requires(array != null);
			Contract.Requires(mask != null);

			if (mask.Shape != array.Shape) throw new MArrayShapeException("The logical indexing mask must have the same shape as the array.");

			var values = new List<TScalar>();
			for (int i = 0; i < mask.Count; ++i)
				if (mask[i])
					values.Add(array[i]);

			return MFullArray<TScalar>.CreateColumnVector(values.ToArray());
		}

		public static void Subsasgn<TScalar>(MArray<TScalar> array, MArray<double> indices, MArray<TScalar> values)
		{
			Contract.Requires(array != null);
			Contract.Requires(indices != null);

			if (indices.IsEmpty && values.IsEmpty) return;

			var indicesShape = indices.Shape;
			Contract.Assert(indicesShape.ColumnCount == 1);
			Contract.Assert(indicesShape == values.Shape);
			
			for (int i = 0; i < indicesShape.RowCount; ++i)
				array[ToInt(indices[i]) - 1] = values[i];
		}
		#endregion
	}
}
