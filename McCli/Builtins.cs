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
		#region Additive
		public static MArray<double> plus(MArray<double> lhs, MArray<double> rhs)
		{
			Contract.Requires(lhs != null);
			Contract.Requires(rhs != null);

			if (lhs.IsScalar) return plus(rhs, lhs[0]);
			if (rhs.IsScalar) return plus(lhs, rhs[0]);
			if (lhs.Shape != rhs.Shape) throw new MArrayShapeException();

			var c = new MDenseArray<double>(lhs.Shape);
			for (int i = 0; i < lhs.Count; ++i)
				c[i] = lhs[i] + rhs[i];
			return c;
		}

		private static MArray<double> plus(MArray<double> lhs, double rhs)
		{
			Contract.Requires(lhs != null);

			var c = new MDenseArray<double>(lhs.Shape);
			for (int i = 0; i < lhs.Count; ++i)
				c[i] = lhs[i] + rhs;
			return c;
		}

		public static MArray<double> minus(MArray<double> lhs, MArray<double> rhs)
		{
			Contract.Requires(lhs != null);
			Contract.Requires(rhs != null);

			if (lhs.IsScalar) return minus(lhs[0], rhs);
			if (rhs.IsScalar) return minus(lhs, rhs[0]);
			if (lhs.Shape != rhs.Shape) throw new MArrayShapeException();

			var c = new MDenseArray<double>(lhs.Shape);
			for (int i = 0; i < lhs.Count; ++i)
				c[i] = lhs[i] - rhs[i];
			return c;
		}

		private static MArray<double> minus(MArray<double> lhs, double rhs)
		{
			Contract.Requires(lhs != null);

			var c = new MDenseArray<double>(lhs.Shape);
			for (int i = 0; i < lhs.Count; ++i)
				c[i] = lhs[i] - rhs;
			return c;
		}

		private static MArray<double> minus(double lhs, MArray<double> rhs)
		{
			Contract.Requires(rhs != null);

			var c = new MDenseArray<double>(rhs.Shape);
			for (int i = 0; i < rhs.Count; ++i)
				c[i] = lhs - rhs[i];
			return c;
		}

		public static MArray<TScalar> uplus<TScalar>(MArray<TScalar> array)
		{
			Contract.Requires(array != null);
			return array.DeepClone();
		}

		public static MDenseArray<double> uminus(MDenseArray<double> array)
		{
			Contract.Requires(array != null);

			var result = new MDenseArray<double>(array.Shape);
			for (int i = 0; i < array.Count; ++i)
				result[i] = -array[i];
			return result;
		}
		#endregion

		#region Multiplicative
		public static MDenseArray<double> times(MArray<double> lhs, MArray<double> rhs)
		{
			Contract.Requires(lhs != null);
			Contract.Requires(rhs != null);

			if (lhs.IsScalar) return times(rhs, lhs[0]);
			if (rhs.IsScalar) return times(lhs, rhs[0]);
			if (lhs.Shape != rhs.Shape) throw new MArrayShapeException();

			var result = new MDenseArray<double>(lhs.Shape);
			for (int i = 0; i < lhs.Count; ++i)
				result[i] = lhs[i] * rhs[i];
			return result;
		}

		private static MDenseArray<double> times(MArray<double> lhs, double rhs)
		{
			Contract.Requires(lhs != null);

			var result = new MDenseArray<double>(lhs.Shape);
			for (int i = 0; i < lhs.Count; ++i)
				result[i] = lhs[i] * rhs;
			return result;
		}

		public static MDenseArray<double> mtimes(MArray<double> lhs, MArray<double> rhs)
		{
			Contract.Requires(lhs != null);
			Contract.Requires(rhs != null);

			var lhsShape = lhs.Shape;
			if (lhsShape.Rank > 2) throw new MArrayShapeException();

			var rhsShape = rhs.Shape;
			if (rhsShape.Rank > 2) throw new MArrayShapeException();

			if (lhsShape.IsScalar) return times(rhs, lhs[0]);
			if (rhsShape.IsScalar) return times(lhs, rhs[0]);
			if (lhsShape.ColumnCount != rhsShape.RowCount) throw new MArrayShapeException();

			var resultShape = new MArrayShape(lhsShape.RowCount, rhsShape.ColumnCount);
			var result = new MDenseArray<double>(resultShape);

			for (int column = 0; column < resultShape.ColumnCount; ++column)
			{
				for (int row = 0; row < resultShape.RowCount; ++row)
				{
					double value = 0;
					for (int i = 0; i < lhsShape.ColumnCount; ++i)
						value += lhs[row + i * resultShape.RowCount] * rhs[i + column * resultShape.RowCount];
					result[row + column * resultShape.RowCount] = value;
				}
			}

			return result;
		}

		public static MDenseArray<double> rdivide(MArray<double> lhs, MArray<double> rhs)
		{
			Contract.Requires(lhs != null);
			Contract.Requires(rhs != null);
			
			if (lhs.IsScalar) return rdivide(lhs[0], rhs);
			if (rhs.IsScalar) return rdivide(lhs, rhs[0]);
			if (lhs.Shape != rhs.Shape) throw new MArrayShapeException();

			var result = new MDenseArray<double>(lhs.Shape);
			for (int i = 0; i < lhs.Count; ++i)
				result[i] = lhs[i] / rhs[i];
			return result;
		}

		private static MDenseArray<double> rdivide(MArray<double> lhs, double rhs)
		{
			Contract.Requires(lhs != null);

			var result = new MDenseArray<double>(lhs.Shape);
			for (int i = 0; i < lhs.Count; ++i)
				result[i] = lhs[i] / rhs;
			return result;
		}

		private static MDenseArray<double> rdivide(double lhs, MArray<double> rhs)
		{
			Contract.Requires(rhs != null);

			var result = new MDenseArray<double>(rhs.Shape);
			for (int i = 0; i < rhs.Count; ++i)
				result[i] = lhs / rhs[i];
			return result;
		}

		public static MDenseArray<double> ldivide(MArray<double> lhs, MArray<double> rhs)
		{
			Contract.Requires(lhs != null);
			Contract.Requires(rhs != null);

			return rdivide(rhs, lhs);
		}
		#endregion

		#region Power
		public static MDenseArray<double> power(MArray<double> @base, MArray<double> exponent)
		{
			Contract.Requires(@base != null);
			Contract.Requires(exponent != null);

			if (@base.IsScalar) 
			if (exponent.IsScalar) return power(@base, exponent[0]);

			var result = new MDenseArray<double>(@base.Shape);
			for (int i = 0; i < @base.Count; ++i)
				result[i] = Math.Pow(@base[i], exponent[i]);
			return result;
		}

		private static MDenseArray<double> power(MArray<double> @base, double exponent)
		{
			Contract.Requires(@base != null);

			var result = new MDenseArray<double>(@base.Shape);
			for (int i = 0; i < @base.Count; ++i)
				result[i] = Math.Pow(@base[i], exponent);
			return result;
		}

		private static MDenseArray<double> power(double @base, MArray<double> exponent)
		{
			Contract.Requires(exponent != null);

			var result = new MDenseArray<double>(exponent.Shape);
			for (int i = 0; i < exponent.Count; ++i)
				result[i] = Math.Pow(@base, exponent[i]);
			return result;
		}
		#endregion
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
		public static MComplex<TReal> complex<[AnyRealNumeric] TReal>(TReal a) where TReal : struct
		{
			return new MComplex<TReal>(a);
		}

		public static MComplex<TReal> complex<[AnyRealNumeric] TReal>(TReal a, TReal b) where TReal : struct
		{
			return new MComplex<TReal>(a, b);
		}

		public static MComplex<double> i()
		{
			return new MComplex<double>(0, 1);
		}

		public static MComplex<double> j()
		{
			return new MComplex<double>(0, 1);
		}

		public static MArray<TReal> real<[AnyRealNumeric] TReal>(MArray<MComplex<TReal>> array) where TReal : struct
		{
			Contract.Requires(array != null);

			var result = new MDenseArray<TReal>(array.Shape);
			for (int i = 0; i < array.Count; ++i)
				result[i] = array[i].RealPart;
			return result;
		}

		public static MArray<TReal> imag<[AnyRealNumeric] TReal>(MArray<MComplex<TReal>> array) where TReal : struct
		{
			Contract.Requires(array != null);

			var result = new MDenseArray<TReal>(array.Shape);
			for (int i = 0; i < array.Count; ++i)
				result[i] = array[i].ImaginaryPart;
			return result;
		}
		#endregion
	}
}
