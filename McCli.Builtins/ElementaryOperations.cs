using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace McCli.Builtins
{
	/// <summary>
	/// Implements MatLab builtins for elementary operations.
	/// http://www.mathworks.com/help/matlab/operators-and-elementary-operations.html
	/// </summary>
	[MatlabLibrary]
	public static class ElementaryOperations
	{
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

		internal static MFullArray<double> plus(MArray<double> lhs, double rhs)
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

		internal static MFullArray<double> minus(MArray<double> lhs, double rhs)
		{
			Contract.Requires(lhs != null);

			var c = new MFullArray<double>(lhs.Shape);
			for (int i = 0; i < lhs.Count; ++i)
				c[i] = lhs[i] - rhs;
			return c;
		}

		internal static MFullArray<double> minus(double lhs, MArray<double> rhs)
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

		#region Elementwise Multiplicative
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

		internal static MFullArray<double> times(MArray<double> lhs, double rhs)
		{
			Contract.Requires(lhs != null);

			var result = new MFullArray<double>(lhs.Shape);
			for (int i = 0; i < lhs.Count; ++i)
				result[i] = lhs[i] * rhs;
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

		internal static MFullArray<double> rdivide(MArray<double> lhs, double rhs)
		{
			Contract.Requires(lhs != null);

			var result = new MFullArray<double>(lhs.Shape);
			for (int i = 0; i < lhs.Count; ++i)
				result[i] = lhs[i] / rhs;
			return result;
		}

		internal static MFullArray<double> rdivide(double lhs, MArray<double> rhs)
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

		#region Matrix Operations
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

		internal static double mtimes(double lhs, double rhs)
		{
			return lhs * rhs;
		}

		public static MFullArray<double> mpower(MArray<double> @base, double exponent)
		{
			Contract.Requires(@base != null);

			if (@base.IsScalar) return mpower(@base[0], exponent);

			throw new NotImplementedException("Non-scalar mpower.");
		}

		internal static double mpower(double @base, double exponent)
		{
			return power(@base, exponent);
		}

		public static MFullArray<double> mrdivide(MArray<double> b, MArray<double> a)
		{
			Contract.Requires(b != null);
			Contract.Requires(a != null);

			if (a.IsHigherDimensional || b.IsHigherDimensional || a.ColumnCount != b.ColumnCount)
				throw new MArrayShapeException();

			if (a.IsScalar && b.IsScalar) return mrdivide(b[0], a[0]);

			throw new NotImplementedException("Non-scalar mrdivide.");
		}
		
		internal static double mrdivide(double b, double a)
		{
			return b / a;
		}

		public static MArray<double> mldivide(MArray<double> a, MArray<double> b)
		{
			Contract.Requires(b != null);
			Contract.Requires(a != null);

			if (a.IsHigherDimensional || b.IsHigherDimensional || a.RowCount != b.RowCount)
				throw new MArrayShapeException();

			if (a.IsScalar && b.IsScalar) return mldivide(a[0], b[0]);

			throw new NotImplementedException("Non-scalar mldivide.");
		}

		internal static double mldivide(double a, double b)
		{
			return b / a;
		}

		public static MArray<double> mpower(MArray<double> @base, double exponent)
		{
			Contract.Requires(@base != null);

			if (!@base.IsSquareMatrix) throw new MArrayShapeException();
			if (@base.IsScalar) return mpower(@base[0], exponent);

			throw new NotImplementedException("Non-scalar mpower.");
		}

		internal static double mpower(double @base, double exponent)
		{
			return power(@base, exponent);
		}
		#endregion

		#region Power
		public static MFullArray<double> power(MArray<double> @base, MArray<double> exponent)
		{
			Contract.Requires(@base != null);
			Contract.Requires(exponent != null);

			if (@base.IsScalar) return power(@base[0], exponent);
			if (exponent.IsScalar) return power(@base, exponent[0]);

			var result = new MFullArray<double>(@base.Shape);
			for (int i = 0; i < @base.Count; ++i)
				result[i] = power(@base[i], exponent[i]);
			return result;
		}

		internal static MFullArray<double> power(MArray<double> @base, double exponent)
		{
			Contract.Requires(@base != null);

			var result = new MFullArray<double>(@base.Shape);
			for (int i = 0; i < @base.Count; ++i)
				result[i] = power(@base[i], exponent);
			return result;
		}

		internal static MFullArray<double> power(double @base, MArray<double> exponent)
		{
			Contract.Requires(exponent != null);

			var result = new MFullArray<double>(exponent.Shape);
			for (int i = 0; i < exponent.Count; ++i)
				result[i] = power(@base, exponent[i]);
			return result;
		}

		internal static double power(double @base, double exponent)
		{
			return Math.Pow(@base, exponent);
		}
		#endregion

		#region Rounding
		public static MArray<double> floor(MArray<double> array)
		{
			Contract.Requires(array != null);

			var result = new MFullArray<double>(array.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = floor(array[i]);
			return result;
		}

		internal static double floor(double value)
		{
			return Math.Floor(value);
		}

		public static MArray<double> ceil(MArray<double> array)
		{
			Contract.Requires(array != null);

			var result = new MFullArray<double>(array.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = ceil(array[i]);
			return result;
		}

		internal static double ceil(double value)
		{
			return Math.Ceiling(value);
		}

		public static MArray<double> round(MArray<double> array)
		{
			Contract.Requires(array != null);

			var result = new MFullArray<double>(array.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = round(array[i]);
			return result;
		}

		internal static double round(double value)
		{
			// Unfortunately, portable class libraries do not have
			// Math.Round(..., MidpointRounding, AwayFromZero).

			if (value > 0)
				return (value % 1) == 0.5 ? ceil(value) : floor(value);
			else
				return (value % 1) == -0.5 ? floor(value) : ceil(value);
		}

		public static MArray<double> fix(MArray<double> array)
		{
			Contract.Requires(array != null);

			var result = new MFullArray<double>(array.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = fix(array[i]);
			return result;
		}

		internal static double fix(double value)
		{
			// Unfortunately, portable class libraries do not have Math.Truncate
			return double.IsInfinity(value) ? value : (value - (value % 1));
		}
		#endregion

		/// <summary>
		/// Checks that two arrays have matching shapes.
		/// If either is a scalar value, it gets expanded to the size of the other.
		/// </summary>
		/// <typeparam name="TScalar">The type of scalar values.</typeparam>
		/// <param name="first">The first array.</param>
		/// <param name="second">The second array</param>
		internal static void MatchShapes<TScalar>(ref MArray<TScalar> first, ref MArray<TScalar> second)
		{
			Contract.Requires(first != null);
			Contract.Requires(second != null);

			if (first.Shape == second.Shape) return;

			if (first.IsScalar) first = MFullArray<TScalar>.ExpandScalar(first[0], second.Shape);
			else if (second.IsScalar) second = MFullArray<TScalar>.ExpandScalar(second[0], first.Shape);
			else throw new MArrayShapeException();
		}
	}
}
