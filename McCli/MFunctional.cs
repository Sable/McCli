using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace McCli
{
	/// <summary>
	/// Provides functional programming-style functions over MatLab arrays.
	/// </summary>
	public static class MFunctional
	{
		public static MArray<TOut> Map<TIn, TOut>(this MArray<TIn> array, Func<TIn, TOut> map)
		{
			Contract.Requires(array != null);
			Contract.Requires(map != null);

			var result = new MFullArray<TOut>(array.Shape);
			for (int i = 0; i < array.Count; ++i)
				result[i] = map(array[i]);
			return result;
		}

		public static MArray<TOut> Map<TIn1, TIn2, TOut>(MArray<TIn1> array1, MArray<TIn2> array2, Func<TIn1, TIn2, TOut> map)
		{
			Contract.Requires(array1 != null);
			Contract.Requires(array2 != null);
			Contract.Requires(map != null);

			var shape = array1.Shape;
			if (array2.Shape != array1.Shape) throw new MArrayShapeException();

			var result = new MFullArray<TOut>(shape);
			for (int i = 0; i < shape.Count; ++i)
				result[i] = map(array1[i], array2[i]);
			return result;
		}

		public static MArray<TOut> Zip<TIn1, TIn2, TOut>(
			MArray<TIn1> lhs, MArray<TIn2> rhs, Func<TIn1, TIn2, TOut> zipper, bool allowScalarArgument)
		{
			Contract.Requires(lhs != null);
			Contract.Requires(rhs != null);

			if (lhs.Shape == rhs.Shape)
			{
				var result = new MFullArray<TOut>(lhs.Shape);
				for (int i = 0; i < result.Count; ++i)
					result[i] = zipper(lhs[i], rhs[i]);
				return result;
			}
			else if (lhs.IsScalar)
			{
				var lhsScalar = lhs[0];
				var result = new MFullArray<TOut>(lhs.Shape);
				for (int i = 0; i < result.Count; ++i)
					result[i] = zipper(lhsScalar, rhs[i]);
				return result;
			}
			else if (rhs.IsScalar)
			{
				var rhsScalar = rhs[0];
				var result = new MFullArray<TOut>(lhs.Shape);
				for (int i = 0; i < result.Count; ++i)
					result[i] = zipper(lhs[i], rhsScalar);
				return result;
			}
			else
			{
				throw new MArrayShapeException();
			}
		}

		public static MArray<TResult> CollapseDimension<TSource, TResult>(
			this MArray<TSource> array, Func<TSource, TResult> map, Func<TResult, TResult, TResult> reduce)
		{
			Contract.Requires(array != null);
			return CollapseDimension(array, array.Shape.IndexOfFirstNonSingletonDimension(), map, reduce);
		}

		public static MArray<TResult> CollapseDimension<TSource, TResult>(
			this MArray<TSource> array, int dimensionIndex,
			Func<TSource, TResult> map, Func<TResult, TResult, TResult> reduce)
		{
			Contract.Requires(array != null);
			Contract.Requires(map != null);
			Contract.Requires(reduce != null);

			if (array.IsEmpty) throw new MArrayShapeException();

			var shape = array.Shape;
			int sliceStep, sliceSize;
			shape.GetDimensionStepAndSize(dimensionIndex, out sliceStep, out sliceSize);

			var result = new MFullArray<TResult>(MArrayShape.CollapseDimension(shape, dimensionIndex));
			var resultArray = result.BackingArray;
			for (int sliceIndex = 0; sliceIndex < resultArray.Length; ++sliceIndex)
			{
				TResult aggregator = map(array[sliceIndex * sliceSize]);
				for (int sliceElementIndex = 1; sliceElementIndex < sliceSize; ++sliceElementIndex)
					aggregator = reduce(aggregator, map(array[sliceIndex * sliceSize + sliceElementIndex * sliceStep]));
				resultArray[sliceIndex] = aggregator;
			}

			return result;
		}
	}
}
