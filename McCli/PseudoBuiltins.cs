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
	public static class PseudoBuiltins
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

		public static string CharArrayToString(MArray<char> array)
		{
			Contract.Requires(array != null);

			var shape = array.Shape;
			if (!shape.IsRowVector) throw new MArrayShapeException();

			// TODO: Don't assume a full array
			return new string(((MFullArray<char>)array).BackingArray, 0, shape.ColumnCount);
		}

		public static MFullArray<char> StringToCharArray(string str)
		{
			Contract.Requires(str != null);
			return MFullArray<char>.CreateRowVector(str.ToCharArray());
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

		#region ArrayGet
		public static MArray<TScalar> ArrayGet<TScalar>(MArray<TScalar> array, MArray<double> indices)
		{
			Contract.Requires(array != null);
			Contract.Requires(indices != null);

			var indicesShape = indices.Shape;
			if (indicesShape.IsEmpty) return new MFullArray<TScalar>(MArrayShape.Empty);
			Contract.Assert(indicesShape.ColumnCount == 1);

			var result = new MFullArray<TScalar>(new MArrayShape(indicesShape.RowCount, 1));
			for (int i = 0; i < indicesShape.RowCount; ++i)
				result[i] = ArrayGet(array, ToInt(indices[i]));
			return result;
		}

		public static MArray<TScalar> ArrayGet<TScalar>(MArray<TScalar> array, MArray<bool> mask)
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

		public static MArray<TScalar> ArrayGet<TScalar>(MArray<TScalar> array, MArray<double> rowIndices, MArray<double> columnIndices)
		{
			Contract.Requires(array != null);
			Contract.Requires(rowIndices != null);
			Contract.Requires(columnIndices != null);

			var indexedShape = new MArrayShape(rowIndices.Count, columnIndices.Count);
			var result = new MFullArray<TScalar>(indexedShape);

			for (int i = 0; i < rowIndices.Count; ++i)
			{
				int rowIndex = ToInt(rowIndices[i]);
				if (rowIndex < 1 || rowIndex > array.RowCount)
					throw new IndexOutOfRangeException();

				for (int j = 0; j < columnIndices.Count; ++j)
				{
					int columnIndex = ToInt(columnIndices[i]);
					if (columnIndex < 1 || columnIndex > array.ColumnCount)
						throw new IndexOutOfRangeException();

					result[j * indexedShape.RowCount + i] = array[(columnIndex - 1) * array.RowCount + (columnIndex - 1)];
				}
			}

			return result;
		}

		internal static TScalar ArrayGet<TScalar>(MArray<TScalar> array, double rowIndex, double columnIndex)
		{
			return ArrayGet(array, ToInt(rowIndex), ToInt(columnIndex));
		}

		internal static TScalar ArrayGet<TScalar>(MArray<TScalar> array, int rowIndex, int columnIndex)
		{
			Contract.Requires(array != null);
			return ArrayGet(array, LinearizeIndex(array.Shape, rowIndex, columnIndex));
		}

		internal static TScalar ArrayGet<TScalar>(MArray<TScalar> array, double index)
		{
			return ArrayGet(array, PseudoBuiltins.ToInt(index));
		}

		internal static TScalar ArrayGet<TScalar>(MArray<TScalar> array, int index)
		{
			return array[index - 1];
		}
		#endregion

		#region ArraySet
		public static void ArraySet<TScalar>(MArray<TScalar> array, MArray<double> indices, MArray<TScalar> values)
		{
			Contract.Requires(array != null);
			Contract.Requires(indices != null);
			Contract.Requires(values != null);

			if (indices.IsEmpty && values.IsEmpty) return;

			var indicesShape = indices.Shape;
			Contract.Assert(indicesShape.ColumnCount == 1);
			Contract.Assert(indicesShape == values.Shape);
			
			for (int i = 0; i < indicesShape.RowCount; ++i)
				array[ToInt(indices[i]) - 1] = values[i];
		}

		public static void ArraySet<TScalar>(MArray<TScalar> array, MArray<double> rowIndices, MArray<double> columnIndices, MArray<TScalar> values)
		{
			Contract.Requires(array != null);
			Contract.Requires(rowIndices != null);
			Contract.Requires(columnIndices != null);
			Contract.Requires(values != null);

			var indexedShape = new MArrayShape(rowIndices.Count, columnIndices.Count);
			if (values.Shape != indexedShape) throw new MArrayShapeException();

			for (int i = 0; i < rowIndices.Count; ++i)
			{
				int rowIndex = ToInt(rowIndices[i]);
				if (rowIndex < 1 || rowIndex > array.RowCount)
					throw new IndexOutOfRangeException();

				for (int j = 0; j < columnIndices.Count; ++j)
				{
					int columnIndex = ToInt(columnIndices[i]);
					if (columnIndex < 1 || columnIndex > array.ColumnCount)
						throw new IndexOutOfRangeException();

					array[(columnIndex - 1) * array.RowCount + (columnIndex - 1)] = values[j * indexedShape.RowCount + i];
				}
			}
		}

		internal static void ArraySet<TScalar>(MArray<TScalar> array, double rowIndex, double columnIndex, TScalar value)
		{
			ArraySet(array, ToInt(rowIndex), ToInt(columnIndex), value);
		}

		internal static void ArraySet<TScalar>(MArray<TScalar> array, int rowIndex, int columnIndex, TScalar value)
		{
			Contract.Requires(array != null);

			var index = LinearizeIndex(array.Shape, rowIndex, columnIndex);
			array[index - 1] = value;
		}
		#endregion

		#region LinearizeIndex
		public static int LinearizeIndex(MArrayShape shape, int rowIndex, int columnIndex)
		{
			if (rowIndex < 1 || rowIndex > shape.RowCount
				|| columnIndex < 1 || columnIndex > shape.ColumnCount)
				throw new ArgumentOutOfRangeException();

			return rowIndex + (columnIndex - 1) * shape.RowCount;
		}
		#endregion
	}
}
