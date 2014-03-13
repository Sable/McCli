using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Builtins
{
	/// <summary>
	/// Implements MatLab builtins for array creation and concatenation.
	/// http://www.mathworks.com/help/matlab/elementary-matrices-and-arrays.html
	/// </summary>
	[MatlabLibrary]
	public static class ArrayCreation
	{
		// TODO: numerical coercion for those (can call zeros(int16(4)))

		#region zeros
		public static double zeros()
		{
			return 0;
		}

		public static MFullArray<double> zeros(double n)
		{
			return zeros(n, n);
		}

		public static MFullArray<double> zeros(double rowCount, double columnCount)
		{
			return MFullArray<double>.CreateWithShape(rowCount, columnCount);
		}

		public static MFullArray<double> zeros(double sz1, double sz2, double sz3)
		{
			var shape = MArrayShape.FromDoubles(sz1, sz2, sz3);
			return new MFullArray<double>(shape);
		}
		#endregion

		#region ones
		public static double ones()
		{
			return 1;
		}

		public static MFullArray<double> ones(double n)
		{
			return ones(n, n);
		}

		public static MFullArray<double> ones(double rowCount, double columnCount)
		{
			return MFullArray<double>.ExpandScalar(1, rowCount, columnCount);
		}
		#endregion

		#region eye
		public static double eye()
		{
			return 1;
		}

		public static MFullArray<double> eye(double n)
		{
			return eye(n, n);
		}

		public static MFullArray<double> eye(double sz1, double sz2)
		{
			var result = zeros(sz1, sz2);
			var array = result.BackingArray;
			for (int i = 0; i < array.Length; ++i)
				array[i] = (i / result.RowCount) == (i % result.RowCount) ? 1 : 0;
			return result;
		}
		#endregion

		#region true
		[BuiltinCilOpcode(0x17 /* ldc.i4.1 */)]
		public static bool @true() { return true; }

		public static MFullArray<bool> @true(double n)
		{
			return @true(n, n);
		}

		public static MFullArray<bool> @true(double rowCount, double columnCount)
		{
			return PseudoBuiltins.Expand(true, rowCount, columnCount);
		}
		#endregion

		#region false
		[BuiltinCilOpcode(0x16 /* ldc.i4.0 */)]
		public static bool @false() { return false; }

		public static MFullArray<bool> @false(double n)
		{
			return @false(n, n);
		}

		public static MFullArray<bool> @false(double rowCount, double columnCount)
		{
			return MFullArray<bool>.CreateWithShape(rowCount, columnCount);
		}
		#endregion

		#region horzcat
		public static MFullArray<TScalar> horzcat<[AnyPrimitive]TScalar>(MFullArray<TScalar> left, MFullArray<TScalar> right)
		{
			Contract.Requires(left != null);
			Contract.Requires(right != null);

			if (left.IsHigherDimensional || right.IsHigherDimensional)
				throw new NotImplementedException("horzcat on higher-dimensional arrays.");

			if (left.RowCount != right.RowCount)
				throw new MArrayShapeException();

			int rowCount = left.RowCount;
			var resultShape = new MArrayShape(rowCount, left.ColumnCount + right.ColumnCount);
			var result = new MFullArray<TScalar>(resultShape);

			for (int rowIndex = 0; rowIndex < rowCount; ++rowIndex)
			{
				for (int leftColumnIndex = 0; leftColumnIndex < left.ColumnCount; ++leftColumnIndex)
					result[leftColumnIndex * rowCount + rowIndex] = left[leftColumnIndex * rowCount + rowIndex];
				for (int rightColumnIndex = 0; rightColumnIndex < right.ColumnCount; ++rightColumnIndex)
					result[(left.ColumnCount + rightColumnIndex) * rowCount + rowIndex] = right[rightColumnIndex * rowCount + rowIndex];
			}

			return result;
		}
		
		// TODO: remove when variadic inputs are implemented
		public static MFullArray<TScalar> horzcat<[AnyPrimitive] TScalar>(
			TScalar value1, TScalar value2)
		{
			return MFullArray<TScalar>.CreateRowVector(value1, value2);
		}

		public static MFullArray<TScalar> horzcat<[AnyPrimitive] TScalar>(
			TScalar value1, TScalar value2, TScalar value3)
		{
			return MFullArray<TScalar>.CreateRowVector(value1, value2, value3);
		}

		public static MFullArray<TScalar> horzcat<[AnyPrimitive] TScalar>(
			TScalar value1, TScalar value2, TScalar value3, TScalar value4)
		{
			return MFullArray<TScalar>.CreateRowVector(value1, value2, value3, value4);
		}

		public static MFullArray<TScalar> horzcat<[AnyPrimitive] TScalar>(
			TScalar value1, TScalar value2, TScalar value3, TScalar value4, TScalar value5)
		{
			return MFullArray<TScalar>.CreateRowVector(value1, value2, value3, value4, value5);
		}
		
		public static MFullArray<TScalar> horzcat<[AnyPrimitive]TScalar>(
			MFullArray<TScalar> array1, MFullArray<TScalar> array2, MFullArray<TScalar> array3)
		{
			return horzcat(new[] { array1, array2, array3 });
		}

		public static MFullArray<TScalar> horzcat<[AnyPrimitive]TScalar>(
			MFullArray<TScalar> array1, MFullArray<TScalar> array2, MFullArray<TScalar> array3, MFullArray<TScalar> array4)
		{
			return horzcat(new[] { array1, array2, array3, array4 });
		}

		public static MFullArray<TScalar> horzcat<[AnyPrimitive]TScalar>(
			MFullArray<TScalar> array1, MFullArray<TScalar> array2, MFullArray<TScalar> array3,
			MFullArray<TScalar> array4, MFullArray<TScalar> array5)
		{
			return horzcat(new[] { array1, array2, array3, array4, array5 });
		}

		public static MFullArray<TScalar> horzcat<[AnyPrimitive]TScalar>(
			MFullArray<TScalar> array1, MFullArray<TScalar> array2, MFullArray<TScalar> array3,
			MFullArray<TScalar> array4, MFullArray<TScalar> array5, MFullArray<TScalar> array6)
		{
			return horzcat(new[] { array1, array2, array3, array4, array5, array6 });
		}

		internal static MFullArray<TScalar> horzcat<[AnyPrimitive]TScalar>(params MFullArray<TScalar>[] arrays)
		{
			Contract.Requires(arrays != null);

			if (arrays.Length == 0) return MFullArray<TScalar>.CreateEmpty();

			var rowCount = arrays[0].RowCount;
			int columnCount = 0;
			foreach (var array in arrays)
			{
				if (array.IsHigherDimensional)
					throw new NotImplementedException("horzcat on higher-dimensional arrays.");

				if (array.RowCount != rowCount)
					throw new MArrayShapeException();

				columnCount += array.ColumnCount;
			}

			var resultShape = new MArrayShape(rowCount, columnCount);
			var result = new MFullArray<TScalar>(resultShape);

			int resultIndex = 0;
			foreach (var array in arrays)
			{
				for (int arrayColumnIndex = 0; arrayColumnIndex < array.ColumnCount; ++arrayColumnIndex)
				{
					for (int rowIndex = 0; rowIndex < rowCount; ++rowIndex)
					{
						result[resultIndex] = array[arrayColumnIndex * rowCount + rowIndex];
						resultIndex++;
					}
				}
			}

			return result;
		}
		#endregion

		#region vertcat
		public static MFullArray<TScalar> vertcat<[AnyPrimitive] TScalar>(
			TScalar value1, TScalar value2)
		{
			return MFullArray<TScalar>.CreateColumnVector(value1, value2);
		}

		public static MFullArray<TScalar> vertcat<[AnyPrimitive] TScalar>(
			TScalar value1, TScalar value2, TScalar value3)
		{
			return MFullArray<TScalar>.CreateColumnVector(value1, value2, value3);
		}

		public static MFullArray<TScalar> vertcat<[AnyPrimitive] TScalar>(
			TScalar value1, TScalar value2, TScalar value3, TScalar value4)
		{
			return MFullArray<TScalar>.CreateColumnVector(value1, value2, value3, value4);
		}

		public static MFullArray<TScalar> vertcat<[AnyPrimitive] TScalar>(
			TScalar value1, TScalar value2, TScalar value3, TScalar value4, TScalar value5)
		{
			return MFullArray<TScalar>.CreateColumnVector(value1, value2, value3, value4, value5);
		}
		#endregion

		// TODO: diag, pascal, magic
	}
}
