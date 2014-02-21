using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McCli.Builtins
{
	/// <summary>
	/// Implements MatLab builtins for common math functions.
	/// </summary>
	[MatlabLibrary]
	public static class CommonMath
	{
		public static MArray<double> abs(MArray<double> array)
		{
			return Arrays.arrayfun(abs, array);
		}

		private static double abs(double value)
		{
			return Math.Abs(value);
		}

		#region Roots/Powers/Exponentials
		public static MArray<double> sqrt(MArray<double> array)
		{
			return Arrays.arrayfun(sqrt, array);
		}

		private static double sqrt(double value)
		{
			return Math.Sqrt(value);
		}

		public static MArray<double> realsqrt(MArray<double> array)
		{
			return Arrays.arrayfun(realsqrt, array);
		}

		private static double realsqrt(double value)
		{
			return Math.Sqrt(value);
		}

		public static MArray<double> exp(MArray<double> array)
		{
			return Arrays.arrayfun(exp, array);
		}

		private static double exp(double value)
		{
			return Math.Exp(value);
		}

		public static MArray<double> pow2(MArray<double> array)
		{
			return Arrays.arrayfun(pow2, array);
		}

		private static double pow2(double value)
		{
			return Math.Pow(2, value);
		}

		public static MArray<double> realpow(MArray<double> @base, MArray<double> exponent)
		{
			return Arrays.arrayfun(realpow, @base, exponent);
		}

		private static double realpow(double @base, double exponent)
		{
			return Math.Pow(@base, exponent);
		}
		#endregion

		#region Logarithms
		public static MArray<double> log(MArray<double> array)
		{
			return Arrays.arrayfun(log, array);
		}

		private static double log(double value)
		{
			return Math.Log(value);
		}

		public static MArray<double> log2(MArray<double> array)
		{
			return Arrays.arrayfun(log2, array);
		}

		private static double log2(double value)
		{
			return Math.Log(value, 2);
		}

		public static MArray<double> log10(MArray<double> array)
		{
			return Arrays.arrayfun(log10, array);
		}

		private static double log10(double value)
		{
			return Math.Log10(value);
		}

		public static MArray<double> reallog(MArray<double> array)
		{
			return Arrays.arrayfun(reallog, array);
		}

		private static double reallog(double value)
		{
			return Math.Log(value);
		}
		#endregion

		// TODO: mod, rem, min, max
	}
}
