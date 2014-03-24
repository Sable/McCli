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

		#region Shameful overloads due to lack of variadic functions
		public static MFullArray<TScalar> horzcat<[AnyPrimitive]TScalar>(
			MFullArray<TScalar> array1, MFullArray<TScalar> array2, MFullArray<TScalar> array3,
			MFullArray<TScalar> array4, MFullArray<TScalar> array5, MFullArray<TScalar> array6,
			MFullArray<TScalar> array7, MFullArray<TScalar> array8, MFullArray<TScalar> array9,
			MFullArray<TScalar> array10, MFullArray<TScalar> array11, MFullArray<TScalar> array12,
			MFullArray<TScalar> array13, MFullArray<TScalar> array14)
		{
			return horzcat(new[] { array1, array2, array3, array4, array5, array6, array7, array8, array9, array10, array11, array12, array13, array14 });
		}

		public static MFullArray<TScalar> horzcat<[AnyPrimitive]TScalar>(
			MFullArray<TScalar> array1, MFullArray<TScalar> array2, MFullArray<TScalar> array3,
			MFullArray<TScalar> array4, MFullArray<TScalar> array5, MFullArray<TScalar> array6,
			MFullArray<TScalar> array7, MFullArray<TScalar> array8, MFullArray<TScalar> array9,
			MFullArray<TScalar> array10, MFullArray<TScalar> array11, MFullArray<TScalar> array12,
			MFullArray<TScalar> array13, MFullArray<TScalar> array14, MFullArray<TScalar> array15,
			MFullArray<TScalar> array16)
		{
			return horzcat(new[]
			{
				array1, array2, array3, array4, array5, array6, array7, array8, array9, array10,
				array11, array12, array13, array14, array15, array16
			});
		}

		public static MFullArray<TScalar> horzcat<[AnyPrimitive]TScalar>(
			MFullArray<TScalar> array1, MFullArray<TScalar> array2, MFullArray<TScalar> array3,
			MFullArray<TScalar> array4, MFullArray<TScalar> array5, MFullArray<TScalar> array6,
			MFullArray<TScalar> array7, MFullArray<TScalar> array8, MFullArray<TScalar> array9,
			MFullArray<TScalar> array10, MFullArray<TScalar> array11, MFullArray<TScalar> array12,
			MFullArray<TScalar> array13, MFullArray<TScalar> array14, MFullArray<TScalar> array15,
			MFullArray<TScalar> array16, MFullArray<TScalar> array17, MFullArray<TScalar> array18,
			MFullArray<TScalar> array19, MFullArray<TScalar> array20, MFullArray<TScalar> array21,
			MFullArray<TScalar> array22, MFullArray<TScalar> array23, MFullArray<TScalar> array24,
			MFullArray<TScalar> array25, MFullArray<TScalar> array26, MFullArray<TScalar> array27,
			MFullArray<TScalar> array28, MFullArray<TScalar> array29, MFullArray<TScalar> array30,
			MFullArray<TScalar> array31, MFullArray<TScalar> array32, MFullArray<TScalar> array33)
		{
			return horzcat(new[]
			{
				array1, array2, array3, array4, array5, array6, array7, array8, array9, array10,
				array11, array12, array13, array14, array15, array16, array17, array18, array19, array20,
				array21, array22, array23, array24, array25, array26, array27, array28, array29, array30,
				array31, array32, array33
			});
		}

		public static MFullArray<TScalar> horzcat<[AnyPrimitive]TScalar>(
			MFullArray<TScalar> array1, MFullArray<TScalar> array2, MFullArray<TScalar> array3,
			MFullArray<TScalar> array4, MFullArray<TScalar> array5, MFullArray<TScalar> array6,
			MFullArray<TScalar> array7, MFullArray<TScalar> array8, MFullArray<TScalar> array9,
			MFullArray<TScalar> array10, MFullArray<TScalar> array11, MFullArray<TScalar> array12,
			MFullArray<TScalar> array13, MFullArray<TScalar> array14, MFullArray<TScalar> array15,
			MFullArray<TScalar> array16, MFullArray<TScalar> array17, MFullArray<TScalar> array18,
			MFullArray<TScalar> array19, MFullArray<TScalar> array20, MFullArray<TScalar> array21,
			MFullArray<TScalar> array22, MFullArray<TScalar> array23, MFullArray<TScalar> array24,
			MFullArray<TScalar> array25, MFullArray<TScalar> array26, MFullArray<TScalar> array27,
			MFullArray<TScalar> array28, MFullArray<TScalar> array29, MFullArray<TScalar> array30,
			MFullArray<TScalar> array31, MFullArray<TScalar> array32, MFullArray<TScalar> array33,
			MFullArray<TScalar> array34, MFullArray<TScalar> array35, MFullArray<TScalar> array36,
			MFullArray<TScalar> array37, MFullArray<TScalar> array38, MFullArray<TScalar> array39,
			MFullArray<TScalar> array40, MFullArray<TScalar> array41, MFullArray<TScalar> array42,
			MFullArray<TScalar> array43, MFullArray<TScalar> array44, MFullArray<TScalar> array45,
			MFullArray<TScalar> array46, MFullArray<TScalar> array47, MFullArray<TScalar> array48,
			MFullArray<TScalar> array49, MFullArray<TScalar> array50, MFullArray<TScalar> array51,
			MFullArray<TScalar> array52, MFullArray<TScalar> array53, MFullArray<TScalar> array54,
			MFullArray<TScalar> array55)
		{
			return horzcat(new[]
			{
				array1, array2, array3, array4, array5, array6, array7, array8, array9, array10,
				array11, array12, array13, array14, array15, array16, array17, array18, array19, array20,
				array21, array22, array23, array24, array25, array26, array27, array28, array29, array30,
				array31, array32, array33, array34, array35, array36, array37, array38, array39, array40,
				array41, array42, array43, array44, array45, array46, array47, array48, array49, array50,
				array51, array52, array53, array54, array55
			});
		}

		public static MFullArray<TScalar> horzcat<[AnyPrimitive]TScalar>(
			MFullArray<TScalar> array1, MFullArray<TScalar> array2, MFullArray<TScalar> array3,
			MFullArray<TScalar> array4, MFullArray<TScalar> array5, MFullArray<TScalar> array6,
			MFullArray<TScalar> array7, MFullArray<TScalar> array8, MFullArray<TScalar> array9,
			MFullArray<TScalar> array10, MFullArray<TScalar> array11, MFullArray<TScalar> array12,
			MFullArray<TScalar> array13, MFullArray<TScalar> array14, MFullArray<TScalar> array15,
			MFullArray<TScalar> array16, MFullArray<TScalar> array17, MFullArray<TScalar> array18,
			MFullArray<TScalar> array19, MFullArray<TScalar> array20, MFullArray<TScalar> array21,
			MFullArray<TScalar> array22, MFullArray<TScalar> array23, MFullArray<TScalar> array24,
			MFullArray<TScalar> array25, MFullArray<TScalar> array26, MFullArray<TScalar> array27,
			MFullArray<TScalar> array28, MFullArray<TScalar> array29, MFullArray<TScalar> array30,
			MFullArray<TScalar> array31, MFullArray<TScalar> array32, MFullArray<TScalar> array33,
			MFullArray<TScalar> array34, MFullArray<TScalar> array35, MFullArray<TScalar> array36,
			MFullArray<TScalar> array37, MFullArray<TScalar> array38, MFullArray<TScalar> array39,
			MFullArray<TScalar> array40, MFullArray<TScalar> array41, MFullArray<TScalar> array42,
			MFullArray<TScalar> array43, MFullArray<TScalar> array44, MFullArray<TScalar> array45,
			MFullArray<TScalar> array46, MFullArray<TScalar> array47, MFullArray<TScalar> array48,
			MFullArray<TScalar> array49, MFullArray<TScalar> array50, MFullArray<TScalar> array51,
			MFullArray<TScalar> array52, MFullArray<TScalar> array53, MFullArray<TScalar> array54,
			MFullArray<TScalar> array55, MFullArray<TScalar> array56)
		{
			return horzcat(new[]
			{
				array1, array2, array3, array4, array5, array6, array7, array8, array9, array10,
				array11, array12, array13, array14, array15, array16, array17, array18, array19, array20,
				array21, array22, array23, array24, array25, array26, array27, array28, array29, array30,
				array31, array32, array33, array34, array35, array36, array37, array38, array39, array40,
				array41, array42, array43, array44, array45, array46, array47, array48, array49, array50,
				array51, array52, array53, array54, array55, array56
			});
		}
		#endregion

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
		public static MFullArray<TScalar> vertcat<[AnyPrimitive] TScalar>(MFullArray<TScalar> array)
		{
			return array.DeepClone();
		}

		public static MFullArray<TScalar> vertcat<[AnyPrimitive] TScalar>(
			MFullArray<TScalar> array1, MFullArray<TScalar> array2)
		{
			return vertcat(new[] { array1, array2 });
		}

		public static MFullArray<TScalar> vertcat<[AnyPrimitive] TScalar>(
			MFullArray<TScalar> array1, MFullArray<TScalar> array2, MFullArray<TScalar> array3)
		{
			return vertcat(new[] { array1, array2, array3 });
		}

		public static MFullArray<TScalar> vertcat<[AnyPrimitive] TScalar>(
			MFullArray<TScalar> array1, MFullArray<TScalar> array2, MFullArray<TScalar> array3, MFullArray<TScalar> array4)
		{
			return vertcat(new[] { array1, array2, array3, array4 });
		}

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

		private static MFullArray<TScalar> vertcat<[AnyPrimitive] TScalar>(params MFullArray<TScalar>[] arrays)
		{
			Contract.Requires(arrays != null);
			Contract.Requires(arrays.Length > 0);

			int columnCount = arrays[0].ColumnCount;
			int totalRowCount = 0;
			foreach (var array in arrays)
			{
				if (array.IsHigherDimensional) throw new NotImplementedException("vertcat on higher-dimensional arrays.");
				totalRowCount += array.RowCount;
			}

			var result = new MFullArray<TScalar>(totalRowCount, columnCount);
			int baseRowIndex = 0;
			foreach (var array in arrays)
			{
				int arrayRowCount = array.RowCount;
				for (int i = 0; i < arrayRowCount; ++i)
					for (int j = 0; j < columnCount; ++j)
						result[j * totalRowCount + baseRowIndex + i] = array[j * arrayRowCount + i];
				baseRowIndex += arrayRowCount;
			}

			return result;
		}
		#endregion

		// TODO: diag, pascal, magic
	}
}
