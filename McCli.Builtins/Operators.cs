using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Builtins
{
	/// <summary>
	/// Implements MatLab operators builtins.
	/// </summary>
	[MatlabLibrary]
	public static class Operators
	{
		#region Arithmetic
		#region Additive
		public static MFullArray<double> plus(MArray<double> lhs, MArray<double> rhs)
		{
			Contract.Requires(lhs != null);
			Contract.Requires(rhs != null);

			if (lhs.IsScalar) return plus(rhs, lhs[0]);
			if (rhs.IsScalar) return plus(lhs, rhs[0]);
			if (lhs.Shape != rhs.Shape) throw new MArrayShapeException();

			var c = new MFullArray<double>(lhs.Shape);
			for (int i = 0; i < lhs.Count; ++i)
				c[i] = lhs[i] + rhs[i];
			return c;
		}

		private static MFullArray<double> plus(MArray<double> lhs, double rhs)
		{
			Contract.Requires(lhs != null);

			var c = new MFullArray<double>(lhs.Shape);
			for (int i = 0; i < lhs.Count; ++i)
				c[i] = lhs[i] + rhs;
			return c;
		}

		public static MFullArray<double> minus(MArray<double> lhs, MArray<double> rhs)
		{
			Contract.Requires(lhs != null);
			Contract.Requires(rhs != null);

			if (lhs.IsScalar) return minus(lhs[0], rhs);
			if (rhs.IsScalar) return minus(lhs, rhs[0]);
			if (lhs.Shape != rhs.Shape) throw new MArrayShapeException();

			var c = new MFullArray<double>(lhs.Shape);
			for (int i = 0; i < lhs.Count; ++i)
				c[i] = lhs[i] - rhs[i];
			return c;
		}

		private static MFullArray<double> minus(MArray<double> lhs, double rhs)
		{
			Contract.Requires(lhs != null);

			var c = new MFullArray<double>(lhs.Shape);
			for (int i = 0; i < lhs.Count; ++i)
				c[i] = lhs[i] - rhs;
			return c;
		}

		private static MFullArray<double> minus(double lhs, MArray<double> rhs)
		{
			Contract.Requires(rhs != null);

			var c = new MFullArray<double>(rhs.Shape);
			for (int i = 0; i < rhs.Count; ++i)
				c[i] = lhs - rhs[i];
			return c;
		}

		public static MArray<TScalar> uplus<TScalar>(MArray<TScalar> array)
		{
			Contract.Requires(array != null);
			return array.DeepClone();
		}

		public static MArray<double> uminus(MArray<double> array)
		{
			Contract.Requires(array != null);

			var result = new MFullArray<double>(array.Shape);
			for (int i = 0; i < array.Count; ++i)
				result[i] = -array[i];
			return result;
		}
		#endregion

		#region Multiplicative
		public static MFullArray<double> times(MArray<double> lhs, MArray<double> rhs)
		{
			Contract.Requires(lhs != null);
			Contract.Requires(rhs != null);

			if (lhs.IsScalar) return times(rhs, lhs[0]);
			if (rhs.IsScalar) return times(lhs, rhs[0]);
			if (lhs.Shape != rhs.Shape) throw new MArrayShapeException();

			var result = new MFullArray<double>(lhs.Shape);
			for (int i = 0; i < lhs.Count; ++i)
				result[i] = lhs[i] * rhs[i];
			return result;
		}

		private static MFullArray<double> times(MArray<double> lhs, double rhs)
		{
			Contract.Requires(lhs != null);

			var result = new MFullArray<double>(lhs.Shape);
			for (int i = 0; i < lhs.Count; ++i)
				result[i] = lhs[i] * rhs;
			return result;
		}

		public static MFullArray<double> mtimes(MArray<double> lhs, MArray<double> rhs)
		{
			Contract.Requires(lhs != null);
			Contract.Requires(rhs != null);

			var lhsShape = lhs.Shape;
			if (lhsShape.DimensionCount > 2) throw new MArrayShapeException();

			var rhsShape = rhs.Shape;
			if (rhsShape.DimensionCount > 2) throw new MArrayShapeException();

			if (lhsShape.IsScalar) return times(rhs, lhs[0]);
			if (rhsShape.IsScalar) return times(lhs, rhs[0]);
			if (lhsShape.ColumnCount != rhsShape.RowCount) throw new MArrayShapeException();

			var resultShape = new MArrayShape(lhsShape.RowCount, rhsShape.ColumnCount);
			var result = new MFullArray<double>(resultShape);

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

		public static MFullArray<double> rdivide(MArray<double> lhs, MArray<double> rhs)
		{
			Contract.Requires(lhs != null);
			Contract.Requires(rhs != null);

			if (lhs.IsScalar) return rdivide(lhs[0], rhs);
			if (rhs.IsScalar) return rdivide(lhs, rhs[0]);
			if (lhs.Shape != rhs.Shape) throw new MArrayShapeException();

			var result = new MFullArray<double>(lhs.Shape);
			for (int i = 0; i < lhs.Count; ++i)
				result[i] = lhs[i] / rhs[i];
			return result;
		}

		private static MFullArray<double> rdivide(MArray<double> lhs, double rhs)
		{
			Contract.Requires(lhs != null);

			var result = new MFullArray<double>(lhs.Shape);
			for (int i = 0; i < lhs.Count; ++i)
				result[i] = lhs[i] / rhs;
			return result;
		}

		private static MFullArray<double> rdivide(double lhs, MArray<double> rhs)
		{
			Contract.Requires(rhs != null);

			var result = new MFullArray<double>(rhs.Shape);
			for (int i = 0; i < rhs.Count; ++i)
				result[i] = lhs / rhs[i];
			return result;
		}

		public static MFullArray<double> ldivide(MArray<double> lhs, MArray<double> rhs)
		{
			Contract.Requires(lhs != null);
			Contract.Requires(rhs != null);

			return rdivide(rhs, lhs);
		}
		#endregion

		#region Power
		public static MFullArray<double> power(MArray<double> @base, MArray<double> exponent)
		{
			Contract.Requires(@base != null);
			Contract.Requires(exponent != null);

			if (@base.IsScalar)
				if (exponent.IsScalar) return power(@base, exponent[0]);

			var result = new MFullArray<double>(@base.Shape);
			for (int i = 0; i < @base.Count; ++i)
				result[i] = Math.Pow(@base[i], exponent[i]);
			return result;
		}

		private static MFullArray<double> power(MArray<double> @base, double exponent)
		{
			Contract.Requires(@base != null);

			var result = new MFullArray<double>(@base.Shape);
			for (int i = 0; i < @base.Count; ++i)
				result[i] = Math.Pow(@base[i], exponent);
			return result;
		}

		private static MFullArray<double> power(double @base, MArray<double> exponent)
		{
			Contract.Requires(exponent != null);

			var result = new MFullArray<double>(exponent.Shape);
			for (int i = 0; i < exponent.Count; ++i)
				result[i] = Math.Pow(@base, exponent[i]);
			return result;
		}
		#endregion

		#region Transpose
		public static MArray<TScalar> transpose<[AnyPrimitive] TScalar>(MArray<TScalar> array)
		{
			Contract.Requires(array != null);
			if (array.IsHigherDimensional) throw new MArrayShapeException();

			var shape = array.Shape;
			var result = new MFullArray<TScalar>(shape.ColumnCount, shape.RowCount);
			for (int row = 0; row < shape.RowCount; ++row)
				for (int column = 0; column < shape.ColumnCount; ++column)
					result[row * shape.ColumnCount + column] = array[column * shape.RowCount + row];
			return result;
		}

		private static TScalar transpose<[AnyPrimitive] TScalar>(TScalar value)
		{
			return value;
		}
		#endregion

		// TODO: matrix multiplication, solves, transpose
		// TODO: vector dot and cross products (not in this class)
		#endregion

		#region Comparison
		public static MArray<bool> eq(MArray<double> a, MArray<double> b)
		{
			CheckMatchingShapes_ScalarExpand(ref a, ref b);

			var result = new MFullArray<bool>(a.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = a[i] == b[i];
			return result;
		}

		public static MArray<bool> ne(MArray<double> a, MArray<double> b)
		{
			CheckMatchingShapes_ScalarExpand(ref a, ref b);

			var result = new MFullArray<bool>(a.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = a[i] != b[i];
			return result;
		}

		public static MArray<bool> gt(MArray<double> a, MArray<double> b)
		{
			CheckMatchingShapes_ScalarExpand(ref a, ref b);

			var result = new MFullArray<bool>(a.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = a[i] > b[i];
			return result;
		}

		public static MArray<bool> lt(MArray<double> a, MArray<double> b)
		{
			CheckMatchingShapes_ScalarExpand(ref a, ref b);

			var result = new MFullArray<bool>(a.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = a[i] < b[i];
			return result;
		}

		public static MArray<bool> ge(MArray<double> a, MArray<double> b)
		{
			CheckMatchingShapes_ScalarExpand(ref a, ref b);

			var result = new MFullArray<bool>(a.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = a[i] >= b[i];
			return result;
		}

		public static MArray<bool> le(MArray<double> a, MArray<double> b)
		{
			CheckMatchingShapes_ScalarExpand(ref a, ref b);

			var result = new MFullArray<bool>(a.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = a[i] <= b[i];
			return result;
		}

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
		public static MFullArray<double> colon(double low, double high)
		{
			if (low > high) return new MFullArray<double>(1, 0);

			var count = (int)(high - low) + 1;
			var array = new MFullArray<double>(1, count);
			for (int i = 0; i < count; ++i)
				array[i] = low + i;

			return array;
		}
		#endregion

		#region Private utilities
		private static void CheckMatchingShapes_ScalarExpand<TScalar>(ref MArray<TScalar> first, ref MArray<TScalar> second)
		{
			Contract.Requires(first != null);
			Contract.Requires(second != null);

			if (first.Shape == second.Shape) return;

			if (first.IsScalar) first = MFullArray<TScalar>.ExpandScalar(first[0], second.Shape);
			else if (second.IsScalar) second = MFullArray<TScalar>.ExpandScalar(second[0], first.Shape);
			else throw new MArrayShapeException();
		}
		#endregion
	}
}
