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
	public static class Floats
	{
		#region Rounding
		public static MArray<double> floor(MArray<double> array)
		{
			Contract.Requires(array != null);

			var result = new MFullArray<double>(array.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = Math.Floor(array[i]);
			return result;
		}

		public static MArray<double> ceil(MArray<double> array)
		{
			Contract.Requires(array != null);

			var result = new MFullArray<double>(array.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = Math.Ceiling(array[i]);
			return result;
		}

		// TODO: Reimplement round and fix now that this is a portable class library
		// (no Math.Truncate or MidpointRounding.AwayFromZero).
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
	}
}
