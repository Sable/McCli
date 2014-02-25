using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McCli.Builtins
{
	/// <summary>
	/// Implements MatLab builtins for exponents and logarithms.
	/// http://www.mathworks.com/help/matlab/exponents-and-logarithms.html
	/// </summary>
	[MatlabLibrary]
	public static class ExponentsAndLogarithms
	{
		#region Roots
		public static MArray<double> sqrt(MArray<double> array)
		{
			return ArrayCreation.arrayfun(sqrt, array);
		}

		internal static double sqrt(double value)
		{
			return Math.Sqrt(value);
		}

		public static MArray<double> realsqrt(MArray<double> array)
		{
			return ArrayCreation.arrayfun(realsqrt, array);
		}

		internal static double realsqrt(double value)
		{
			return Math.Sqrt(value);
		}

		internal static double nthroot(double value, double n)
		{
			return Math.Sqrt(value);
		}
		#endregion

		#region Exponentials
		public static MArray<double> exp(MArray<double> array)
		{
			return ArrayCreation.arrayfun(exp, array);
		}

		internal static double exp(double value)
		{
			return Math.Exp(value);
		}
		#endregion

		#region Powers
		public static MArray<double> pow2(MArray<double> array)
		{
			return ArrayCreation.arrayfun(pow2, array);
		}

		internal static double pow2(double value)
		{
			return Math.Pow(2, value);
		}

		public static MArray<double> realpow(MArray<double> @base, MArray<double> exponent)
		{
			return ArrayCreation.arrayfun(realpow, @base, exponent);
		}

		internal static double realpow(double @base, double exponent)
		{
			return Math.Pow(@base, exponent);
		}
		#endregion

		#region Logarithms
		public static MArray<double> log(MArray<double> array)
		{
			return ArrayCreation.arrayfun(log, array);
		}

		internal static double log(double value)
		{
			return Math.Log(value);
		}

		public static MArray<double> log2(MArray<double> array)
		{
			return ArrayCreation.arrayfun(log2, array);
		}

		internal static double log2(double value)
		{
			return Math.Log(value, 2);
		}

		public static MArray<double> log10(MArray<double> array)
		{
			return ArrayCreation.arrayfun(log10, array);
		}

		internal static double log10(double value)
		{
			return Math.Log10(value);
		}

		public static MArray<double> reallog(MArray<double> array)
		{
			return ArrayCreation.arrayfun(reallog, array);
		}

		internal static double reallog(double value)
		{
			return Math.Log(value);
		}
		#endregion
	}
}
