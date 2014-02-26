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
		public static MArray<double> min(MArray<double> array)
		{
			Contract.Requires(array != null);

			if (array.IsEmpty) return MFullArray<double>.CreateEmpty();

			// TODO: handle NaNs
			if (array.IsRowVector || array.IsColumnVector)
			{
				double min = double.PositiveInfinity;
				for (int i = 0; i < array.RowCount; ++i)
					if (array[i] < min)
						min = array[i];
				return min;
			}

			if (array.IsHigherDimensional) throw new NotImplementedException();

			var result = new MFullArray<double>(1, array.ColumnCount);
			for (int columnIndex = 0; columnIndex < array.ColumnCount; ++columnIndex)
			{
				double min = double.PositiveInfinity;
				for (int rowIndex = 0; rowIndex < array.RowCount; ++rowIndex)
					if (array[columnIndex * array.RowCount + rowIndex] < min)
						min = array[columnIndex];
				result[columnIndex] = min;
			}
			return result;
		}

		public static MArray<double> max(MArray<double> array)
		{
			Contract.Requires(array != null);

			if (array.IsEmpty) return MFullArray<double>.CreateEmpty();

			// TODO: handle NaNs
			if (array.IsRowVector || array.IsColumnVector)
			{
				double max = double.NegativeInfinity;
				for (int i = 0; i < array.RowCount; ++i)
					if (array[i] > max)
						max = array[i];
				return max;
			}

			if (array.IsHigherDimensional) throw new NotImplementedException();

			var result = new MFullArray<double>(1, array.ColumnCount);
			for (int columnIndex = 0; columnIndex < array.ColumnCount; ++columnIndex)
			{
				double max = double.NegativeInfinity;
				for (int rowIndex = 0; rowIndex < array.RowCount; ++rowIndex)
					if (array[columnIndex * array.RowCount + rowIndex] > max)
						max = array[columnIndex];
				result[columnIndex] = max;
			}

			return result;
		}
	}
}
