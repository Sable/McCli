using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace McCli.Builtins
{
	/// <summary>
	/// Implements MatLab builtins for logical operations.
	/// http://www.mathworks.com/help/matlab/logical-operators.html
	/// </summary>
	[MatlabLibrary]
	public sealed class LogicalOperations
	{
		#region and
		public static MArray<bool> and(MArray<bool> a, MArray<bool> b)
		{
			Contract.Requires(a != null);
			Contract.Requires(b != null);

			if (a.IsScalar) return and(b, a[0]);
			if (b.IsScalar) return and(a, b[0]);
			if (a.Shape != b.Shape) throw new MArrayShapeException();

			return ArrayCreation.arrayfun(and, a, b);
		}

		internal static MArray<bool> and(MArray<bool> a, bool b)
		{
			return b ? a.DeepClone() : new MFullArray<bool>(a.Shape);
		}

		internal static bool and(bool a, bool b)
		{
			return a && b;
		}
		#endregion

		#region or
		public static MArray<bool> or(MArray<bool> a, MArray<bool> b)
		{
			Contract.Requires(a != null);
			Contract.Requires(b != null);

			if (a.IsScalar) return or(b, a[0]);
			if (b.IsScalar) return or(a, b[0]);
			if (a.Shape != b.Shape) throw new MArrayShapeException();

			return ArrayCreation.arrayfun(or, a, b);
		}

		internal static MArray<bool> or(MArray<bool> a, bool b)
		{
			return b ? MFullArray<bool>.ExpandScalar(true, a.Shape) : a.DeepClone();
		}

		internal static bool or(bool a, bool b)
		{
			return a || b;
		}
		#endregion

		#region xor
		public static MArray<bool> xor(MArray<bool> a, MArray<bool> b)
		{
			Contract.Requires(a != null);
			Contract.Requires(b != null);

			if (a.IsScalar) return xor(b, a[0]);
			if (b.IsScalar) return xor(a, b[0]);
			if (a.Shape != b.Shape) throw new MArrayShapeException();

			return ArrayCreation.arrayfun(xor, a, b);
		}

		internal static MArray<bool> xor(MArray<bool> a, bool b)
		{
			return b ? not(a) : a.DeepClone();
		}

		internal static bool xor(bool a, bool b)
		{
			return !a ^ !b;
		}
		#endregion

		#region not
		public static MArray<bool> not(MArray<bool> array)
		{
			Contract.Requires(array != null);

			if (array.IsScalar) return not(array[0]);
			return ArrayCreation.arrayfun(not, array);
		}

		internal static bool not(bool value)
		{
			return !value;
		}
		#endregion

		#region any
		public static MArray<bool> any(MArray<bool> array)
		{
			Contract.Requires(array != null);

			if (array.IsEmpty) return false;

			var shape = array.Shape;
			int dimensionIndex = shape.IndexOfFirstNonSingletonDimension();
			int sliceStep, sliceSize;
			shape.GetDimensionStepAndSize(dimensionIndex, out sliceStep, out sliceSize);

			var result = new MFullArray<bool>(MArrayShape.CollapseDimension(shape, dimensionIndex));
			var resultArray = result.BackingArray;
			for (int sliceIndex = 0; sliceIndex < resultArray.Length; ++sliceIndex)
			{
				bool value = false;
				for (int sliceElementIndex = 0; sliceElementIndex < sliceSize; ++sliceElementIndex)
					value |= array[sliceIndex * sliceSize + sliceElementIndex * sliceStep];
				resultArray[sliceIndex] = value;
			}

			return result;
		}
		#endregion

		#region all
		public static MArray<bool> all(MArray<bool> array)
		{
			Contract.Requires(array != null);

			if (array.IsEmpty) return true;

			var shape = array.Shape;
			int dimensionIndex = shape.IndexOfFirstNonSingletonDimension();
			int sliceStep, sliceSize;
			shape.GetDimensionStepAndSize(dimensionIndex, out sliceStep, out sliceSize);

			var result = new MFullArray<bool>(MArrayShape.CollapseDimension(shape, dimensionIndex));
			var resultArray = result.BackingArray;
			for (int sliceIndex = 0; sliceIndex < resultArray.Length; ++sliceIndex)
			{
				bool value = true;
				for (int sliceElementIndex = 0; sliceElementIndex < sliceSize; ++sliceElementIndex)
					value &= array[sliceIndex * sliceSize + sliceElementIndex * sliceStep];
				resultArray[sliceIndex] = value;
			}

			return result;
		}
		#endregion
	}
}
