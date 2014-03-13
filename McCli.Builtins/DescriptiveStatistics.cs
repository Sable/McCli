using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace McCli.Builtins
{
	/// <summary>
	/// Implements MatLab builtins for descriptive statistics.
	/// http://www.mathworks.com/help/matlab/descriptive-statistics.html
	/// </summary>
	[MatlabLibrary]
	public static class DescriptiveStatistics
	{
		public static MFullArray<double> min(MFullArray<double> array)
		{
			Contract.Requires(array != null);

			var shape = array.Shape;
			int dimensionIndex = shape.IndexOfFirstNonSingletonDimension();
			int sliceStep, sliceSize;
			shape.GetDimensionStepAndSize(dimensionIndex, out sliceStep, out sliceSize);
			
			var result = new MFullArray<double>(MArrayShape.CollapseDimension(shape, dimensionIndex));
			var resultArray = result.BackingArray;
			for (int sliceIndex = 0; sliceIndex < resultArray.Length; ++sliceIndex)
			{
				double minimum = double.PositiveInfinity;
				for (int sliceElementIndex = 0; sliceElementIndex < sliceSize; ++sliceElementIndex)
				{
					double value = array[sliceIndex * sliceSize + sliceElementIndex * sliceStep];
					if (value < minimum) minimum = value;
				}

				resultArray[sliceIndex] = minimum;
			}

			return result;
		}

		public static MFullArray<double> max(MFullArray<double> array)
		{
			Contract.Requires(array != null);

			var shape = array.Shape;
			int dimensionIndex = shape.IndexOfFirstNonSingletonDimension();
			int sliceStep, sliceSize;
			shape.GetDimensionStepAndSize(dimensionIndex, out sliceStep, out sliceSize);

			var result = new MFullArray<double>(MArrayShape.CollapseDimension(shape, dimensionIndex));
			var resultArray = result.BackingArray;
			for (int sliceIndex = 0; sliceIndex < resultArray.Length; ++sliceIndex)
			{
				double maximum = double.NegativeInfinity;
				for (int sliceElementIndex = 0; sliceElementIndex < sliceSize; ++sliceElementIndex)
				{
					double value = array[sliceIndex * sliceSize + sliceElementIndex * sliceStep];
					if (value > maximum) maximum = value;
				}

				resultArray[sliceIndex] = maximum;
			}

			return result;
		}

		public static MFullArray<double> mean(MFullArray<double> array)
		{
			Contract.Requires(array != null);

			var shape = array.Shape;
			int dimensionIndex = shape.IndexOfFirstNonSingletonDimension();
			int sliceStep, sliceSize;
			shape.GetDimensionStepAndSize(dimensionIndex, out sliceStep, out sliceSize);

			var result = new MFullArray<double>(MArrayShape.CollapseDimension(shape, dimensionIndex));
			var resultArray = result.BackingArray;
			for (int sliceIndex = 0; sliceIndex < resultArray.Length; ++sliceIndex)
			{
				double sum = 0;
				for (int sliceElementIndex = 0; sliceElementIndex < sliceSize; ++sliceElementIndex)
					sum += array[sliceIndex * sliceSize + sliceElementIndex * sliceStep];

				resultArray[sliceIndex] = sum / sliceSize;
			}

			return result;
		}
	}
}
