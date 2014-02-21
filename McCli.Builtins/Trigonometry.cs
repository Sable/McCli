using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace McCli.Builtins
{
	/// <summary>
	/// Implements MatLab trigonometric builtins
	/// </summary>
	[MatlabLibrary]
	public static class Trigonometry
	{
		public static double pi()
		{
			return Math.PI;
		}

		#region Degrees/Radians
		public static MArray<double> degtorad(MArray<double> array)
		{
			return Arrays.arrayfun(degtorad, array);
		}

		private static double degtorad(double value)
		{
			return value * (Math.PI / 180);
		}

		public static MArray<double> radtodeg(MArray<double> array)
		{
			return Arrays.arrayfun(radtodeg, array);
		}

		private static double radtodeg(double value)
		{
			return value * (180 / Math.PI);
		}
		#endregion

		#region cos, sin, ...
		public static MArray<double> cos(MArray<double> array)
		{
			return Arrays.arrayfun(cos, array);
		}

		private static double cos(double value)
		{
			return Math.Cos(value);
		}

		public static MArray<double> cot(MArray<double> array)
		{
			return Arrays.arrayfun(cot, array);
		}

		private static double cot(double value)
		{
			return 1 / tan(value);
		}

		public static MArray<double> csc(MArray<double> array)
		{
			return Arrays.arrayfun(csc, array);
		}

		private static double csc(double value)
		{
			return 1 / sin(value);
		}

		public static MArray<double> sec(MArray<double> array)
		{
			return Arrays.arrayfun(sec, array);
		}

		private static double sec(double value)
		{
			return 1 / cos(value);
		}

		public static MArray<double> sin(MArray<double> array)
		{
			return Arrays.arrayfun(sin, array);
		}

		private static double sin(double value)
		{
			return Math.Sin(value);
		}

		public static MArray<double> tan(MArray<double> array)
		{
			return Arrays.arrayfun(tan, array);
		}

		private static double tan(double value)
		{
			return Math.Tan(value);
		}
		#endregion

		#region cosd, sind, ...
		public static MArray<double> cosd(MArray<double> array)
		{
			return Arrays.arrayfun(cosd, array);
		}

		private static double cosd(double value)
		{
			return cos(degtorad(value));
		}

		public static MArray<double> cotd(MArray<double> array)
		{
			return Arrays.arrayfun(cotd, array);
		}

		private static double cotd(double value)
		{
			return cot(degtorad(value));
		}

		public static MArray<double> cscd(MArray<double> array)
		{
			return Arrays.arrayfun(cscd, array);
		}

		private static double cscd(double value)
		{
			return csc(degtorad(value));
		}

		public static MArray<double> secd(MArray<double> array)
		{
			return Arrays.arrayfun(secd, array);
		}

		private static double secd(double value)
		{
			return sec(degtorad(value));
		}

		public static MArray<double> sind(MArray<double> array)
		{
			return Arrays.arrayfun(sind, array);
		}

		private static double sind(double value)
		{
			return sin(degtorad(value));
		}

		public static MArray<double> tand(MArray<double> array)
		{
			return Arrays.arrayfun(tand, array);
		}

		private static double tand(double value)
		{
			return tan(degtorad(value));
		}
		#endregion

		#region acos, asin, ...
		public static MArray<double> acos(MArray<double> array)
		{
			return Arrays.arrayfun(acos, array);
		}

		private static double acos(double value)
		{
			return Math.Acos(value);
		}

		public static MArray<double> acot(MArray<double> array)
		{
			return Arrays.arrayfun(acot, array);
		}

		private static double acot(double value)
		{
			return (Math.PI * 0.5) - atan(value);
		}

		public static MArray<double> acsc(MArray<double> array)
		{
			return Arrays.arrayfun(acsc, array);
		}

		private static double acsc(double value)
		{
			return (Math.PI * 0.5) - asec(value);
		}

		public static MArray<double> asec(MArray<double> array)
		{
			return Arrays.arrayfun(asec, array);
		}

		private static double asec(double value)
		{
			return acos(1 / value);
		}

		public static MArray<double> asin(MArray<double> array)
		{
			return Arrays.arrayfun(asin, array);
		}

		private static double asin(double value)
		{
			return Math.Asin(value);
		}

		public static MArray<double> atan(MArray<double> array)
		{
			return Arrays.arrayfun(atan, array);
		}

		private static double atan(double value)
		{
			return Math.Atan(value);
		}
		#endregion

		#region acosd, asind, ...
		public static MArray<double> acosd(MArray<double> array)
		{
			return Arrays.arrayfun(acosd, array);
		}

		private static double acosd(double value)
		{
			return radtodeg(acos(value));
		}

		public static MArray<double> acotd(MArray<double> array)
		{
			return Arrays.arrayfun(acotd, array);
		}

		private static double acotd(double value)
		{
			return radtodeg(acotd(value));
		}

		public static MArray<double> acscd(MArray<double> array)
		{
			return Arrays.arrayfun(acscd, array);
		}

		private static double acscd(double value)
		{
			return radtodeg(acsc(value));
		}

		public static MArray<double> asecd(MArray<double> array)
		{
			return Arrays.arrayfun(asecd, array);
		}

		private static double asecd(double value)
		{
			return radtodeg(asec(value));
		}

		public static MArray<double> asind(MArray<double> array)
		{
			return Arrays.arrayfun(asind, array);
		}

		private static double asind(double value)
		{
			return radtodeg(asin(value));
		}

		public static MArray<double> atand(MArray<double> array)
		{
			return Arrays.arrayfun(atand, array);
		}

		private static double atand(double value)
		{
			return radtodeg(atan(value));
		}
		#endregion

		#region cosh, sinh, ...
		public static MArray<double> cosh(MArray<double> array)
		{
			return Arrays.arrayfun(cosh, array);
		}

		private static double cosh(double value)
		{
			return Math.Cosh(value);
		}

		public static MArray<double> coth(MArray<double> array)
		{
			return Arrays.arrayfun(coth, array);
		}

		private static double coth(double value)
		{
			return cosh(value) / sinh(value);
		}

		public static MArray<double> csch(MArray<double> array)
		{
			return Arrays.arrayfun(csch, array);
		}

		private static double csch(double value)
		{
			return 1 / sinh(value);
		}

		public static MArray<double> sech(MArray<double> array)
		{
			return Arrays.arrayfun(sech, array);
		}

		private static double sech(double value)
		{
			return 1 / cosh(value);
		}

		public static MArray<double> sinh(MArray<double> array)
		{
			return Arrays.arrayfun(sinh, array);
		}

		private static double sinh(double value)
		{
			return Math.Sinh(value);
		}

		public static MArray<double> tanh(MArray<double> array)
		{
			return Arrays.arrayfun(tanh, array);
		}

		private static double tanh(double value)
		{
			return Math.Tanh(value);
		}
		#endregion
		// TODO: All other trig' functions
	}
}
