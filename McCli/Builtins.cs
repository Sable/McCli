using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	public static class Builtins
	{
		#region Arithmetic operators
		public static MArray<double> plus(MArray<double> a, MArray<double> b)
		{
			Contract.Requires(a != null);
			Contract.Requires(b != null);

			if (a.IsScalar) return plus(b, a[0]);
			if (b.IsScalar) return plus(a, b[0]);
			if (a.Shape != b.Shape) throw new MArrayShapeException();

			var c = new MDenseArray<double>(a.Shape);
			for (int i = 0; i < a.Count; ++i)
				c[i] = a[i] + b[i];
			return c;
		}

		private static MArray<double> plus(MArray<double> a, double b)
		{
			Contract.Requires(a != null);

			var c = new MDenseArray<double>(a.Shape);
			for (int i = 0; i < a.Count; ++i)
				c[i] = a[i] + b;
			return c;
		}

		public static MArray<double> minus(MArray<double> a, MArray<double> b)
		{
			Contract.Requires(a != null);
			Contract.Requires(b != null);

			if (a.IsScalar) return minus(a[0], b);
			if (b.IsScalar) return minus(a, b[0]);
			if (a.Shape != b.Shape) throw new MArrayShapeException();

			var c = new MDenseArray<double>(a.Shape);
			for (int i = 0; i < a.Count; ++i)
				c[i] = a[i] - b[i];
			return c;
		}

		private static MArray<double> minus(MArray<double> a, double b)
		{
			Contract.Requires(a != null);

			var c = new MDenseArray<double>(a.Shape);
			for (int i = 0; i < a.Count; ++i)
				c[i] = a[i] - b;
			return c;
		}

		private static MArray<double> minus(double a, MArray<double> b)
		{
			Contract.Requires(b != null);

			var c = new MDenseArray<double>(b.Shape);
			for (int i = 0; i < b.Count; ++i)
				c[i] = a - b[i];
			return c;
		}
		#endregion

		#region Comparison
		public static bool isequal(MArray<double> a, MArray<double> b)
		{
			Contract.Requires(a != null);
			Contract.Requires(b != null);

			if (a.Shape != b.Shape) return false;

			for (int i = 0; i < a.Count; ++i)
				if (a[i] != b[i])
					return false;

			return true;
		}
		#endregion

		#region Colon
		public static MDenseArray<double> colon(double low, double high)
		{
			if (low > high) return new MDenseArray<double>(1, 0);

			var count = (int)(high - low) + 1;
			var array = new MDenseArray<double>(1, count);
			for (int i = 0; i < count; ++i)
				array[i] = low + i;

			return array;
		}
		#endregion

		#region Array Creation
		#region Zeroes
		public static double zeros()
		{
			return 0;
		}

		public static MDenseArray<double> zeros(int n)
		{
			Contract.Requires(n >= 0);
			return zeros(n, n);
		}

		public static MDenseArray<double> zeros(int sz1, int sz2)
		{
			Contract.Requires(sz1 >= 0);
			Contract.Requires(sz2 >= 0);
			return new MDenseArray<double>(sz1, sz2);
		}
		#endregion

		#region Ones
		public static double ones()
		{
			return 1;
		}

		public static MDenseArray<double> ones(int n)
		{
			Contract.Requires(n >= 0);
			return ones(n, n);
		}

		public static MDenseArray<double> ones(int sz1, int sz2)
		{
			Contract.Requires(sz1 >= 0);
			Contract.Requires(sz2 >= 0);

			var result = new MDenseArray<double>(sz1, sz2);
			var array = result.BackingArray;
			for (int i = 0; i < array.Length; ++i)
				array[i] = 1;
			return result;
		}
		#endregion

		#region Eye
		public static double eye()
		{
			return 1;
		}

		public static MDenseArray<double> eye(int n)
		{
			Contract.Requires(n >= 0);
			return eye(n, n);
		}

		public static MDenseArray<double> eye(int sz1, int sz2)
		{
			Contract.Requires(sz1 >= 0);
			Contract.Requires(sz2 >= 0);

			var result = new MDenseArray<double>(sz1, sz2);
			var array = result.BackingArray;
			for (int i = 0; i < array.Length; ++i)
				array[i] = (i / sz1) == (i % sz1) ? 1 : 0;
			return result;
		}
		#endregion
		#endregion

		#region Complex
		public static MComplex<TNumeric> complex<TNumeric>(TNumeric a) where TNumeric : struct
		{
			return new MComplex<TNumeric>(a);
		}

		public static MComplex<TNumeric> complex<TNumeric>(TNumeric a, TNumeric b) where TNumeric : struct
		{
			return new MComplex<TNumeric>(a, b);
		}

		public static MComplex<TNumeric> i<TNumeric>() where TNumeric : struct
		{
			return new MComplex<TNumeric>(default(TNumeric), (TNumeric)Convert.ChangeType(1, typeof(TNumeric)));
		}

		public static MComplex<TNumeric> j<TNumeric>() where TNumeric : struct
		{
			return i<TNumeric>();
		}

		public static MArray<TNumeric> real<TNumeric>(MArray<MComplex<TNumeric>> array) where TNumeric : struct
		{
			Contract.Requires(array != null);

			var result = new MDenseArray<TNumeric>(array.Shape);
			for (int i = 0; i < array.Count; ++i)
				result[i] = array[i].RealPart;
			return result;
		}

		public static MArray<TNumeric> imag<TNumeric>(MArray<MComplex<TNumeric>> array) where TNumeric : struct
		{
			Contract.Requires(array != null);

			var result = new MDenseArray<TNumeric>(array.Shape);
			for (int i = 0; i < array.Count; ++i)
				result[i] = array[i].ImaginaryPart;
			return result;
		}
		#endregion
	}
}
