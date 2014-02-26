using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Builtins
{
	/// <summary>
	/// Implements MatLab builtins related to numeric types.
	/// http://www.mathworks.com/help/matlab/numeric-types.html
	/// </summary>
	[MatlabLibrary]
	public static class NumericTypes
	{
		#region Type Category Testing
		public static bool isfloat(MValue value)
		{
			Contract.Requires(value != null);
			return (value.Class.Kind & MClassKinds.FloatMask) != 0;
		}

		public static bool isinteger(MValue value)
		{
			Contract.Requires(value != null);
			return (value.Class.Kind & MClassKinds.IntegerMask) != 0;
		}

		public static bool isnumeric(MValue value)
		{
			Contract.Requires(value != null);
			return (value.Class.Kind & MClassKinds.NumericMask) != 0;
		}

		public static bool isreal(MValue value)
		{
			Contract.Requires(value != null);
			return !value.Repr.Type.IsComplex;
		}

		// TODO: When overloading is implemented,
		// add concrete-type versions of the is*** type checking builtins above
		#endregion

		#region Float Finiteness Testing
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

		#region Special Constants
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
			var shape = PseudoBuiltins.ToShape(n, m);
			return MFullArray<double>.ExpandScalar(double.NaN, shape);
		}
		#endregion
	}
}
