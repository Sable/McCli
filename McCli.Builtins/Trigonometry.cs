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

		public static MArray<double> hypot(MArray<double> x, MArray<double> y)
		{
			ElementaryOperations.MatchShapes(ref x, ref y);
			var result = new MFullArray<double>(x.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = Math.Sqrt(x[i] * x[i] + y[i] * y[i]);
			return result;
		}

		public static double hypot(double x, double y)
		{
			return Math.Sqrt(x * x + y * y);
		}

		#region Degrees/Radians
		public static MArray<double> degtorad(MArray<double> array)
		{
			return MFunctional.Map(array, degtorad);
		}

		public static double degtorad(double value)
		{
			return value * (Math.PI / 180);
		}

		public static MArray<double> radtodeg(MArray<double> array)
		{
			return MFunctional.Map(array, radtodeg);
		}

		public static double radtodeg(double value)
		{
			return value * (180 / Math.PI);
		}
		#endregion

		#region cos, sin, ...
		public static MArray<double> cos(MArray<double> array)
		{
			return MFunctional.Map(array, cos);
		}

		public static double cos(double value)
		{
			return Math.Cos(value);
		}

		public static MArray<double> cot(MArray<double> array)
		{
			return MFunctional.Map(array, cot);
		}

		public static double cot(double value)
		{
			return 1 / tan(value);
		}

		public static MArray<double> csc(MArray<double> array)
		{
			return MFunctional.Map(array, csc);
		}

		public static double csc(double value)
		{
			return 1 / sin(value);
		}

		public static MArray<double> sec(MArray<double> array)
		{
			return MFunctional.Map(array, sec);
		}

		public static double sec(double value)
		{
			return 1 / cos(value);
		}

		public static MArray<double> sin(MArray<double> array)
		{
			return MFunctional.Map(array, sin);
		}

		public static double sin(double value)
		{
			return Math.Sin(value);
		}

		public static MArray<double> tan(MArray<double> array)
		{
			return MFunctional.Map(array, tan);
		}

		public static double tan(double value)
		{
			return Math.Tan(value);
		}
		#endregion

		#region cosd, sind, ...
		public static MArray<double> cosd(MArray<double> array)
		{
			return MFunctional.Map(array, cosd);
		}

		public static double cosd(double value)
		{
			return cos(degtorad(value));
		}

		public static MArray<double> cotd(MArray<double> array)
		{
			return MFunctional.Map(array, cotd);
		}

		public static double cotd(double value)
		{
			return cot(degtorad(value));
		}

		public static MArray<double> cscd(MArray<double> array)
		{
			return MFunctional.Map(array, cscd);
		}

		public static double cscd(double value)
		{
			return csc(degtorad(value));
		}

		public static MArray<double> secd(MArray<double> array)
		{
			return MFunctional.Map(array, secd);
		}

		public static double secd(double value)
		{
			return sec(degtorad(value));
		}

		public static MArray<double> sind(MArray<double> array)
		{
			return MFunctional.Map(array, sind);
		}

		public static double sind(double value)
		{
			return sin(degtorad(value));
		}

		public static MArray<double> tand(MArray<double> array)
		{
			return MFunctional.Map(array, tand);
		}

		public static double tand(double value)
		{
			return tan(degtorad(value));
		}
		#endregion

		#region acos, asin, ...
		public static MArray<double> acos(MArray<double> array)
		{
			return MFunctional.Map(array, acos);
		}

		public static double acos(double value)
		{
			return Math.Acos(value);
		}

		public static MArray<double> acot(MArray<double> array)
		{
			return MFunctional.Map(array, acot);
		}

		public static double acot(double value)
		{
			return (Math.PI * 0.5) - atan(value);
		}

		public static MArray<double> acsc(MArray<double> array)
		{
			return MFunctional.Map(array, acsc);
		}

		public static double acsc(double value)
		{
			return (Math.PI * 0.5) - asec(value);
		}

		public static MArray<double> asec(MArray<double> array)
		{
			return MFunctional.Map(array, asec);
		}

		public static double asec(double value)
		{
			return acos(1 / value);
		}

		public static MArray<double> asin(MArray<double> array)
		{
			return MFunctional.Map(array, asin);
		}

		public static double asin(double value)
		{
			return Math.Asin(value);
		}

		public static MArray<double> atan(MArray<double> array)
		{
			return MFunctional.Map(array, atan);
		}

		public static double atan(double value)
		{
			return Math.Atan(value);
		}
		#endregion

		#region acosd, asind, ...
		public static MArray<double> acosd(MArray<double> array)
		{
			return MFunctional.Map(array, acosd);
		}

		public static double acosd(double value)
		{
			return radtodeg(acos(value));
		}

		public static MArray<double> acotd(MArray<double> array)
		{
			return MFunctional.Map(array, acotd);
		}

		public static double acotd(double value)
		{
			return radtodeg(acotd(value));
		}

		public static MArray<double> acscd(MArray<double> array)
		{
			return MFunctional.Map(array, acscd);
		}

		public static double acscd(double value)
		{
			return radtodeg(acsc(value));
		}

		public static MArray<double> asecd(MArray<double> array)
		{
			return MFunctional.Map(array, asecd);
		}

		public static double asecd(double value)
		{
			return radtodeg(asec(value));
		}

		public static MArray<double> asind(MArray<double> array)
		{
			return MFunctional.Map(array, asind);
		}

		public static double asind(double value)
		{
			return radtodeg(asin(value));
		}

		public static MArray<double> atand(MArray<double> array)
		{
			return MFunctional.Map(array, atand);
		}

		public static double atand(double value)
		{
			return radtodeg(atan(value));
		}
		#endregion

		#region cosh, sinh, ...
		public static MArray<double> cosh(MArray<double> array)
		{
			return MFunctional.Map(array, cosh);
		}

		public static double cosh(double value)
		{
			return Math.Cosh(value);
		}

		public static MArray<double> coth(MArray<double> array)
		{
			return MFunctional.Map(array, coth);
		}

		public static double coth(double value)
		{
			return cosh(value) / sinh(value);
		}

		public static MArray<double> csch(MArray<double> array)
		{
			return MFunctional.Map(array, csch);
		}

		public static double csch(double value)
		{
			return 1 / sinh(value);
		}

		public static MArray<double> sech(MArray<double> array)
		{
			return MFunctional.Map(array, sech);
		}

		public static double sech(double value)
		{
			return 1 / cosh(value);
		}

		public static MArray<double> sinh(MArray<double> array)
		{
			return MFunctional.Map(array, sinh);
		}

		public static double sinh(double value)
		{
			return Math.Sinh(value);
		}

		public static MArray<double> tanh(MArray<double> array)
		{
			return MFunctional.Map(array, tanh);
		}

		public static double tanh(double value)
		{
			return Math.Tanh(value);
		}
		#endregion
	}
}
