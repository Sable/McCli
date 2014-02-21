using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Builtins
{
	/// <summary>
	/// Implements MatLab builtins which relate to floating-point values.
	/// </summary>
	[MatlabLibrary]
	public static class Floats
	{
		#region Rounding
		public static MArray<double> floor(MArray<double> array)
		{
			Contract.Requires(array != null);

			var result = new MFullArray<double>(array.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = floor(array[i]);
			return result;
		}

		private static double floor(double value)
		{
			return Math.Floor(value);
		}

		public static MArray<double> ceil(MArray<double> array)
		{
			Contract.Requires(array != null);

			var result = new MFullArray<double>(array.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = ceil(array[i]);
			return result;
		}

		private static double ceil(double value)
		{
			return Math.Ceiling(value);
		}

		public static MArray<double> round(MArray<double> array)
		{
			Contract.Requires(array != null);

			var result = new MFullArray<double>(array.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = round(array[i]);
			return result;
		}

		private static double round(double value)
		{
			// Unfortunately, portable class libraries do not have
			// Math.Round(..., MidpointRounding, AwayFromZero).

			if (value > 0)
				return (value % 1) == 0.5 ? ceil(value) : floor(value);
			else
				return (value % 1) == -0.5 ? floor(value) : ceil(value);
		}

		public static MArray<double> fix(MArray<double> array)
		{
			Contract.Requires(array != null);

			var result = new MFullArray<double>(array.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = fix(array[i]);
			return result;
		}

		private static double fix(double value)
		{
			// Unfortunately, portable class libraries do not have Math.Truncate
			return double.IsInfinity(value) ? value : (value - (value % 1));
		}
		#endregion

		#region "Constants"
		public static double NaN()
		{
			return double.NaN;
		}

		public static MArray<double> NaN(double n)
		{
			return NaN(n, n);
		}

		public static MArray<double> NaN(double n, double m)
		{
			var shape = Utilities.ToShape(n, m);
			return MFullArray<double>.ExpandScalar(double.NaN, shape);
		}
		#endregion

		#region Finite/Infinity/NaN testing
		public static MArray<bool> isfinite(MArray<double> array)
		{
			Contract.Requires(array != null);

			var result = new MFullArray<bool>(array.Shape);
			for (int i = 0; i < array.Count; ++i)
				result[i] = !double.IsNaN(array[i]) && !double.IsInfinity(array[i]);
			return result;
		}

		public static MArray<bool> isinf(MArray<double> array)
		{
			Contract.Requires(array != null);

			var result = new MFullArray<bool>(array.Shape);
			for (int i = 0; i < array.Count; ++i)
				result[i] = double.IsInfinity(array[i]);
			return result;
		}

		public static MArray<bool> isnan(MArray<double> array)
		{
			Contract.Requires(array != null);

			var result = new MFullArray<bool>(array.Shape);
			for (int i = 0; i < array.Count; ++i)
				result[i] = double.IsNaN(array[i]);
			return result;
		}
		#endregion

		// eps
	}
}
