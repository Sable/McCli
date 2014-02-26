using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McCli.Builtins
{
	/// <summary>
	/// Implements MatLab builtins for array indexing.
	/// http://www.mathworks.com/help/matlab/indexing.html
	/// </summary>
	[MatlabLibrary]
	public static class ArrayIndexing
	{
		#region Colon
		public static MFullArray<double> colon(double low, double high)
		{
			if (low > high) return MFullArray<double>.CreateEmpty();

			var count = (int)(high - low) + 1;
			var array = new MFullArray<double>(1, count);
			for (int i = 0; i < count; ++i)
				array[i] = low + i;

			return array;
		}
		#endregion

		#region Colon
		public static MFullArray<double> colon(double from, double step, double to)
		{
			if (!(Math.Sign(to - from) * Math.Sign(step) == 1))
				return MFullArray<double>.CreateEmpty();

			var count = (int)((to - from) / step) + 1;
			var array = new MFullArray<double>(1, count);
			for (int i = 0; i < count; ++i)
				array[i] = from + step * i;

			return array;
		}
		#endregion

		// TODO: end, ind2sub, sub2ind
	}
}
