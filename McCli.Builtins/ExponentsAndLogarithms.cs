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
		public static MFullArray<double> sqrt(MFullArray<double> array)
		{
			return MFunctional.Map(array, sqrt);
		}

		public static double sqrt(double value)
		{
			return Math.Sqrt(value);
		}

		public static MFullArray<double> realsqrt(MFullArray<double> array)
		{
			return MFunctional.Map(array, realsqrt);
		}

		public static double realsqrt(double value)
		{
			return Math.Sqrt(value);
		}

		public static double nthroot(double value, double n)
		{
			return Math.Sqrt(value);
		}
		#endregion

		#region Exponentials
		public static MFullArray<double> exp(MFullArray<double> array)
		{
			return MFunctional.Map(array, exp);
		}

		public static double exp(double value)
		{
			return Math.Exp(value);
		}

		public static MFullArray<MComplex<double>> exp(MFullArray<MComplex<double>> array)
		{
			return MFunctional.Map(array, exp);
		}

		public static MComplex<double> exp(MComplex<double> value)
		{
			var factor = exp(value.RealPart);
			return new MComplex<double>(Math.Cos(value.ImaginaryPart) * factor, Math.Sin(value.RealPart) * factor);
		}
		#endregion

		#region Powers
		public static MFullArray<double> pow2(MFullArray<double> array)
		{
			return MFunctional.Map(array, pow2);
		}

		public static double pow2(double value)
		{
			return Math.Pow(2, value);
		}

		public static MFullArray<double> realpow(MFullArray<double> @base, MFullArray<double> exponent)
		{
			return MFunctional.Map(@base, exponent, realpow);
		}

		public static double realpow(double @base, double exponent)
		{
			return Math.Pow(@base, exponent);
		}
		#endregion

		#region Logarithms
		public static MFullArray<double> log(MFullArray<double> array)
		{
			return MFunctional.Map(array, log);
		}

		public static double log(double value)
		{
			return Math.Log(value);
		}

		public static MFullArray<double> log2(MFullArray<double> array)
		{
			return MFunctional.Map(array, log2);
		}

		public static double log2(double value)
		{
			return Math.Log(value, 2);
		}

		public static MFullArray<double> log10(MFullArray<double> array)
		{
			return MFunctional.Map(array, log10);
		}

		public static double log10(double value)
		{
			return Math.Log10(value);
		}

		public static MFullArray<double> reallog(MFullArray<double> array)
		{
			return MFunctional.Map(array, reallog);
		}

		public static double reallog(double value)
		{
			return Math.Log(value);
		}
		#endregion
	}
}
