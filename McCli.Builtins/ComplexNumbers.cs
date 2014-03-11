using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Builtins
{
	/// <summary>
	/// Implements MatLab builtins related to complex/imaginary numbers.
	/// http://www.mathworks.com/help/matlab/complex-numbers.html
	/// </summary>
	[MatlabLibrary]
	public static class ComplexNumbers
	{
		#region abs
		public static MArray<double> abs(MArray<double> array)
		{
			return MFunctional.Map(array, abs);
		}

		public static MArray<double> abs(MArray<MComplex<double>> array)
		{
			return MFunctional.Map(array, abs);
		}

		internal static double abs(double value)
		{
			return Math.Abs(value);
		}

		internal static double abs(MComplex<double> value)
		{
			return Math.Sqrt(value.RealPart * value.RealPart + value.ImaginaryPart * value.ImaginaryPart);
		}
		#endregion

		#region Complex
		public static MComplex<TReal> complex<[AnyReal] TReal>(TReal a) where TReal : struct
		{
			return new MComplex<TReal>(a);
		}

		public static MComplex<TReal> complex<[AnyReal] TReal>(TReal a, TReal b) where TReal : struct
		{
			return new MComplex<TReal>(a, b);
		}

		public static MArray<MComplex<TReal>> complex<[AnyReal] TReal>(MArray<TReal> a) where TReal : struct
		{
			return MFunctional.Map(a, complex);
		}

		public static MArray<MComplex<TReal>> complex<[AnyReal] TReal>(MArray<TReal> a, MArray<TReal> b) where TReal : struct
		{
			return MFunctional.Map(a, b, complex);
		}
		#endregion

		public static MComplex<double> i()
		{
			return new MComplex<double>(0, 1);
		}

		public static MComplex<double> j()
		{
			return new MComplex<double>(0, 1);
		}

		public static MArray<TReal> real<[AnyReal] TReal>(MArray<MComplex<TReal>> array) where TReal : struct
		{
			Contract.Requires(array != null);

			var result = new MFullArray<TReal>(array.Shape);
			for (int i = 0; i < array.Count; ++i)
				result[i] = array[i].RealPart;
			return result;
		}

		public static MArray<TReal> imag<[AnyReal] TReal>(MArray<MComplex<TReal>> array) where TReal : struct
		{
			Contract.Requires(array != null);

			var result = new MFullArray<TReal>(array.Shape);
			for (int i = 0; i < array.Count; ++i)
				result[i] = array[i].ImaginaryPart;
			return result;
		}
	}
}
