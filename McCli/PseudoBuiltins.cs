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
			// Don't do (value % 1) == 0, it's slooow
			if (i != value) throw new InvalidCastException();
			return i;
		}

		public static int ToIntScalar(MFullArray<double> value)
		{
			Contract.Requires(value != null);
			if (!value.IsScalar) throw new MArrayShapeException();
			return ToInt(value[0]);
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
						if (!(doubleArray[i] != 0))
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

		[BuiltinCilOpcode(0x00 /* nop */)]
		public static bool IsTrue(bool value) { return value; }
		public static bool IsTrue(double value) { return value != 0; }

		public static MFullArray<TScalar> Expand<[AnyPrimitive] TScalar>(TScalar value, double rowCount, double columnCount)
		{
			var shape = MArrayShape.FromDoubles(rowCount, columnCount);
			return MFullArray<TScalar>.ExpandScalar(value, shape);
		}

		public static TScalar ToScalar<[AnyPrimitive] TScalar>(MFullArray<TScalar> array)
		{
			Contract.Requires(array != null);
			if (!array.IsScalar) throw new MArrayShapeException();
			return array[0];
		}

		public static MComplex<TReal> ToComplex<[AnyReal] TReal>(TReal value) where TReal : struct
		{
			return new MComplex<TReal>(value, default(TReal));
		}

		public static MFullArray<MComplex<TReal>> ToComplex<[AnyReal] TReal>(MFullArray<TReal> value) where TReal : struct
		{
			return MFunctional.Map(value, ToComplex);
		}

		public static MFullArray<double> ToArray(MIntegralRange<double> range)
		{
			var result = new MFullArray<double>(1, range.Count);
			for (int i = 0; i < range.Count; ++i)
				result[i] = (double)(range.First + i);
			return result;
		}

		#region For Loops
		public static int GetForSliceCount(MValue value)
		{
			Contract.Requires(value != null);
			var shape = value.shape;
			var count = shape.Count;
			return count == 0 ? 0 : (count / shape.RowCount);
		}

		public static MFullArray<TScalar> GetForSlice<[AnyPrimitive] TScalar>(MFullArray<TScalar> array, int index)
		{
			Contract.Requires(array != null);

			var arrayShape = array.shape;
			Contract.Assert(!arrayShape.IsHigherDimensional);

			int baseArrayIndex = arrayShape.RowCount * index;
			var slice = new MFullArray<TScalar>(new MArrayShape(arrayShape.RowCount, 1));
			for (int i = 0; i < arrayShape.RowCount; ++i)
				slice[i] = array[baseArrayIndex + i];
			return slice;
		}
		#endregion

		#region ArrayGet
		public static MFullArray<TScalar> ArrayGet<[AnyPrimitive] TScalar>(MFullArray<TScalar> array, MFullArray<double> indices)
		{
			Contract.Requires(array != null);
			Contract.Requires(indices != null);

			var indicesShape = indices.shape;
			if (indicesShape.IsEmpty) return new MFullArray<TScalar>(MArrayShape.Empty);
			Contract.Assert(indicesShape.ColumnCount == 1);

			var result = new MFullArray<TScalar>(new MArrayShape(indicesShape.RowCount, 1));
			for (int i = 0; i < indicesShape.RowCount; ++i)
				result[i] = ArrayGet(array, ToInt(indices[i]));
			return result;
		}

		public static MFullArray<TScalar> ArrayGet<[AnyPrimitive] TScalar>(MFullArray<TScalar> array, MIntegralRange<double> indices)
		{
			Contract.Requires(array != null);

			if (indices.First < 1 || indices.End > array.Count)
				throw new ArgumentOutOfRangeException("indices");

			var result = new MFullArray<TScalar>(indices.Count, 1);
			Array.Copy(array.BackingArray, indices.First - 1, result.BackingArray, 0, indices.Count);
			return result;
		}

		public static MFullArray<TScalar> ArrayGet<[AnyPrimitive] TScalar>(MFullArray<TScalar> array, MFullArray<bool> mask)
		{
			Contract.Requires(array != null);
			Contract.Requires(mask != null);

			if (mask.shape != array.shape) throw new MArrayShapeException("The logical indexing mask must have the same shape as the array.");

			var values = new List<TScalar>();
			for (int i = 0; i < mask.Count; ++i)
				if (mask[i])
					values.Add(array[i]);

			return MFullArray<TScalar>.CreateColumnVector(values.ToArray());
		}

		public static MFullArray<TScalar> ArrayGet<[AnyPrimitive] TScalar>(
			MFullArray<TScalar> array, MFullArray<double> rowIndices, MFullArray<double> columnIndices)
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

					result[j * indexedShape.RowCount + i] = array[(columnIndex - 1) * array.RowCount + (rowIndex - 1)];
				}
			}

			return result;
		}

		public static TScalar ArrayGet<[AnyPrimitive] TScalar>(MFullArray<TScalar> array, double index)
		{
			// Performance critical method!
			Contract.Requires(array != null);
			int indexInt = ToInt(index);
			if (indexInt > array.shape.Count) // We'll get an IndexOutOfRangeException if it's < 1
				throw new ArgumentOutOfRangeException("index");
			return array.elements[indexInt - 1];
		}

		public static TScalar ArrayGet<[AnyPrimitive] TScalar>(
			MFullArray<TScalar> array, double rowIndex, double columnIndex)
		{
			// Performance critical method!
			Contract.Requires(array != null);

			int rowCount = array.shape.RowCount;

			int rowIndexInt = ToInt(rowIndex);
			if (rowIndexInt < 1 || rowIndexInt > rowCount)
				throw new ArgumentOutOfRangeException("rowIndex");

			int columnIndexInt = ToInt(columnIndex);
			if (columnIndexInt > array.shape.ColumnCount) // We'll get an IndexOutOfRangeException if it's < 1
				throw new ArgumentOutOfRangeException("columnIndex");

			return array.elements[(columnIndexInt - 1) * rowCount + (rowIndexInt - 1)];
		}

		public static TScalar ArrayGet<[AnyPrimitive] TScalar>(
			MFullArray<TScalar> array, double rowIndex, double columnIndex, double sliceIndex)
		{
			Contract.Requires(array != null);
			var index = LinearizeIndex(array.shape, ToInt(rowIndex), ToInt(columnIndex), ToInt(sliceIndex));
			return array.elements[index - 1];
		}
		#endregion

		#region ArraySet
		public static void ArraySet<[AnyPrimitive] TScalar>(
			MFullArray<TScalar> array, MFullArray<TScalar> values)
		{
			Contract.Requires(array != null);
			Contract.Requires(values != null);

			if (values.IsScalar)
			{
				ArraySet(array, values[0]);
			}
			else
			{
				if (values.shape != array.shape) throw new MArrayShapeException();

				int count = array.Count;
				for (int i = 0; i < count; ++i)
					array[i] = values[i];
			}
		}

		public static void ArraySet<[AnyPrimitive] TScalar>(
			MFullArray<TScalar> array, TScalar value)
		{
			Contract.Requires(array != null);
			int count = array.Count;
			for (int i = 0; i < count; ++i)
				array[i] = value;
		}

		public static void ArraySet<[AnyPrimitive] TScalar>(
			MFullArray<TScalar> array, MFullArray<double> indices, MFullArray<TScalar> values)
		{
			Contract.Requires(array != null);
			Contract.Requires(indices != null);
			Contract.Requires(values != null);

			if (indices.IsEmpty && values.IsEmpty) return;

			var indicesShape = indices.shape;
			Contract.Assert(indicesShape.ColumnCount == 1);
			Contract.Assert(indicesShape == values.shape);
			
			for (int i = 0; i < indicesShape.RowCount; ++i)
				array[ToInt(indices[i]) - 1] = values[i];
		}

		public static void ArraySet<[AnyPrimitive] TScalar>(
			MFullArray<TScalar> array, double index, TScalar value)
		{
			Contract.Requires(array != null);
			ArraySet(array, ToInt(index), value);
		}

		public static void ArraySet<[AnyPrimitive] TScalar>(
			MFullArray<TScalar> array, int index, TScalar value)
		{
			Contract.Requires(array != null);

			if (index < 1) throw new ArgumentOutOfRangeException("index");
			if (index > array.Count)
			{
				// Must resize, only works if the array is empty, a scalar or a vector
				// - Empty, scalar or row vector: grows in columns
				// - Column vector: grows in rows
				// - Otherwise: throws
				if (array.IsHigherDimensional) throw new ArgumentOutOfRangeException("index");
				else if (array.RowCount <= 1) array.Resize(new MArrayShape(1, index));
				else if (array.ColumnCount == 1) array.Resize(new MArrayShape(index, 1));
				else throw new ArgumentOutOfRangeException("index");
			}

			array[index - 1] = value;
		}

		public static void ArraySet<[AnyPrimitive] TScalar>(
			MFullArray<TScalar> array, MFullArray<double> rowIndices, MFullArray<double> columnIndices, MFullArray<TScalar> values)
		{
			Contract.Requires(array != null);
			Contract.Requires(rowIndices != null);
			Contract.Requires(columnIndices != null);
			Contract.Requires(values != null);

			var indexedShape = new MArrayShape(rowIndices.Count, columnIndices.Count);
			if (values.shape != indexedShape) throw new MArrayShapeException();

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

					var value = values[j * indexedShape.RowCount + i];
					array[(columnIndex - 1) * array.RowCount + (columnIndex - 1)] = value;
				}
			}
		}

		public static void ArraySet<[AnyPrimitive] TScalar>(
			MFullArray<TScalar> array, double rowIndex, double columnIndex, TScalar value)
		{
			// Performance critical method!
			Contract.Requires(array != null);

			int rowIndexInt = ToInt(rowIndex);
			if (rowIndexInt < 1)
				throw new ArgumentOutOfRangeException("rowIndex");

			int rowCount = array.shape.RowCount;

			int columnIndexInt = ToInt(columnIndex);
			if (rowIndexInt > rowCount || columnIndexInt > array.shape.ColumnCount)
			{
				if (array.shape.IsHigherDimensional) throw new ArgumentOutOfRangeException();
				if (rowIndexInt > rowCount) rowCount = rowIndexInt;
				int columnCount = Math.Max(array.shape.ColumnCount, columnIndexInt);
				array.Resize(new MArrayShape(rowCount, columnCount));
			}

			array.elements[(columnIndexInt - 1) * rowCount + (rowIndexInt - 1)] = value;
		}

		public static void ArraySet<[AnyPrimitive] TScalar>(
			MFullArray<TScalar> array, int rowIndex, int columnIndex, TScalar value)
		{
			Contract.Requires(array != null);

			if (rowIndex < 1) throw new ArgumentOutOfRangeException("rowIndex");
			if (columnIndex < 1) throw new ArgumentOutOfRangeException("columnIndex");

			if (rowIndex > array.RowCount || columnIndex > array.ColumnCount)
			{
				// Array needs resizing
				// TODO: zeros(2, 2, 2) (3, 3) = 5 should work and produce a 3x2x2 matrix
				if (array.IsHigherDimensional) throw new ArgumentOutOfRangeException();
				array.Resize(MArrayShape.Max(array.shape, new MArrayShape(rowIndex, columnIndex)));
			}

			var index = LinearizeIndex(array.shape, rowIndex, columnIndex);
			array[index - 1] = value;
		}

		public static void ArraySet<[AnyPrimitive] TScalar>(
			MFullArray<TScalar> array, double rowIndex, double columnIndex, double sliceIndex, TScalar value)
		{
			var index = LinearizeIndex(array.shape, ToInt(rowIndex), ToInt(columnIndex), ToInt(sliceIndex));
			array[index - 1] = value;
		}
		#endregion

		#region Linearize
		public static MFullArray<TScalar> Linearize<[AnyPrimitive] TScalar>(MFullArray<TScalar> array)
		{
			Contract.Requires(array != null);

			int count = array.Count;
			var result = new MFullArray<TScalar>(count, 1);
			Array.Copy(array.BackingArray, result.BackingArray, count);
			return result;
		}
		#endregion

		#region LinearizeIndex
		internal static int LinearizeIndex(MArrayShape shape, int rowIndex, int columnIndex)
		{
			if (rowIndex < 1 || rowIndex > shape.RowCount
				|| columnIndex < 1 || columnIndex > shape.ColumnCount)
				throw new ArgumentOutOfRangeException();

			return rowIndex + (columnIndex - 1) * shape.RowCount;
		}

		internal static int LinearizeIndex(MArrayShape shape, int rowIndex, int columnIndex, int sliceIndex)
		{
			int sliceCount = shape.GetDimensionSize(2);
			if (rowIndex < 1 || rowIndex > shape.RowCount
				|| columnIndex < 1 || columnIndex > shape.ColumnCount
				|| sliceIndex < 1 || sliceIndex > sliceCount)
				throw new ArgumentOutOfRangeException();

			return rowIndex + ((sliceCount - 1) * shape.ColumnCount + columnIndex - 1) * shape.RowCount;
		}
		#endregion
	}
}
