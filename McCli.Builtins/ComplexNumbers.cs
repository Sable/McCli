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
		public static MFullArray<double> abs(MFullArray<double> array)
		{
			return MFunctional.Map(array, abs);
		}

		public static MFullArray<double> abs(MFullArray<MComplex<double>> array)
		{
			return MFunctional.Map(array, abs);
		}

		public static double abs(double value)
		{
			return Math.Abs(value);
		}

		public static double abs(MComplex<double> value)
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

		public static MFullArray<MComplex<TReal>> complex<[AnyReal] TReal>(MFullArray<TReal> a) where TReal : struct
		{
			return MFunctional.Map(a, complex);
		}

		public static MFullArray<MComplex<TReal>> complex<[AnyReal] TReal>(MFullArray<TReal> a, MFullArray<TReal> b) where TReal : struct
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

		#region real/imag
		public static MFullArray<TReal> real<[AnyReal] TReal>(MFullArray<MComplex<TReal>> array) where TReal : struct
		{
			Contract.Requires(array != null);

			var result = new MFullArray<TReal>(array.Shape);
			for (int i = 0; i < array.Count; ++i)
				result[i] = array[i].RealPart;
			return result;
		}

		public static TReal real<[AnyReal] TReal>(MComplex<TReal> value) where TReal : struct
		{
			return value.RealPart;
		}

		public static MFullArray<TReal> imag<[AnyReal] TReal>(MFullArray<MComplex<TReal>> array) where TReal : struct
		{
			Contract.Requires(array != null);

			var result = new MFullArray<TReal>(array.Shape);
			for (int i = 0; i < array.Count; ++i)
				result[i] = array[i].ImaginaryPart;
			return result;
		}

		public static TReal imag<[AnyReal] TReal>(MComplex<TReal> value) where TReal : struct
		{
			return value.ImaginaryPart;
		}
		#endregion
	}
}
