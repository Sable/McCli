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
		public static MFullArray<bool> and(MFullArray<bool> lhs, MFullArray<bool> rhs)
		{
			Contract.Requires(lhs != null);
			Contract.Requires(rhs != null);

			if (lhs.IsScalar) return and(rhs, lhs[0]);
			if (rhs.IsScalar) return and(lhs, rhs[0]);
			if (lhs.Shape != rhs.Shape) throw new MArrayShapeException();

			return MFunctional.Map(lhs, rhs, and);
		}

		public static MFullArray<bool> and(MFullArray<bool> lhs, bool rhs)
		{
			return rhs ? lhs.DeepClone() : new MFullArray<bool>(lhs.Shape);
		}

		public static MFullArray<bool> and(bool lhs, MFullArray<bool> rhs)
		{
			return and(rhs, lhs);
		}

		public static bool and(bool lhs, bool rhs)
		{
			return lhs && rhs;
		}
		#endregion

		#region or
		public static MFullArray<bool> or(MFullArray<bool> lhs, MFullArray<bool> rhs)
		{
			Contract.Requires(lhs != null);
			Contract.Requires(rhs != null);

			if (lhs.IsScalar) return or(rhs, lhs[0]);
			if (rhs.IsScalar) return or(lhs, rhs[0]);
			if (lhs.Shape != rhs.Shape) throw new MArrayShapeException();

			return MFunctional.Map(lhs, rhs, or);
		}

		public static MFullArray<bool> or(MFullArray<bool> lhs, bool rhs)
		{
			return rhs ? MFullArray<bool>.ExpandScalar(true, lhs.Shape) : lhs.DeepClone();
		}

		public static MFullArray<bool> or(bool lhs, MFullArray<bool> rhs)
		{
			return or(rhs, lhs);
		}

		[BuiltinCilOpcode(0x60 /* or */)]
		public static bool or(bool lhs, bool rhs)
		{
			return lhs | rhs;
		}
		#endregion

		#region xor
		public static MFullArray<bool> xor(MFullArray<bool> lhs, MFullArray<bool> rhs)
		{
			Contract.Requires(lhs != null);
			Contract.Requires(rhs != null);

			if (lhs.IsScalar) return xor(rhs, lhs[0]);
			if (rhs.IsScalar) return xor(lhs, rhs[0]);
			if (lhs.Shape != rhs.Shape) throw new MArrayShapeException();

			return MFunctional.Map(lhs, rhs, xor);
		}

		public static MFullArray<bool> xor(MFullArray<bool> lhs, bool rhs)
		{
			return rhs ? not(lhs) : lhs.DeepClone();
		}

		public static MFullArray<bool> xor(bool lhs, MFullArray<bool> rhs)
		{
			return xor(rhs, lhs);
		}

		public static bool xor(bool lhs, bool rhs)
		{
			return !lhs ^ !rhs;
		}
		#endregion

		#region not
		public static MFullArray<bool> not(MFullArray<bool> array)
		{
			Contract.Requires(array != null);

			if (array.IsScalar) return not(array[0]);
			return MFunctional.Map(array, not);
		}

		[BuiltinCilOpcode(0x66 /* not */)]
		public static bool not(bool value)
		{
			return !value;
		}
		#endregion

		#region any
		public static MFullArray<bool> any(MFullArray<bool> array)
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

		public static MFullArray<bool> any(MFullArray<double> array)
		{
			Contract.Requires(array != null);
			if (array.IsEmpty) return false;
			return MFunctional.CollapseDimension(array, d => d != 0, (b1, b2) => b1 | b2);
		}
		#endregion

		#region all
		public static MFullArray<bool> all(MFullArray<bool> array)
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
