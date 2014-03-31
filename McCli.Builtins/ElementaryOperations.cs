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
		#region Real Additive
		public static MFullArray<double> plus(MFullArray<double> lhs, MFullArray<double> rhs)
		{
			Contract.Requires(lhs != null);
			Contract.Requires(rhs != null);

			if (lhs.IsScalar) return plus(lhs[0], rhs);
			if (rhs.IsScalar) return plus(lhs, rhs[0]);
			if (lhs.Shape != rhs.Shape) throw new MArrayShapeException();

			var c = new MFullArray<double>(lhs.Shape);
			for (int i = 0; i < lhs.Count; ++i)
				c[i] = lhs[i] + rhs[i];
			return c;
		}

		public static MFullArray<double> plus(MFullArray<double> lhs, double rhs)
		{
			Contract.Requires(lhs != null);

			var c = new MFullArray<double>(lhs.Shape);
			for (int i = 0; i < lhs.Count; ++i)
				c[i] = lhs[i] + rhs;
			return c;
		}

		public static MFullArray<double> plus(double lhs, MFullArray<double> rhs)
		{
			return plus(rhs, lhs);
		}

		[BuiltinCilOpcode(0x58 /* add */)]
		public static double plus(double lhs, double rhs)
		{
			return lhs + rhs;
		}

		public static MFullArray<TScalar> uplus<[AnyNumeric]TScalar>(MFullArray<TScalar> array)
		{
			Contract.Requires(array != null);
			return array.DeepClone();
		}

		public static TScalar uplus<[AnyNumeric]TScalar>(TScalar value)
		{
			return value;
		}

		public static MFullArray<double> sum(MFullArray<double> array)
		{
			Contract.Requires(array != null);

			if (array.IsEmpty) return 0;

			var shape = array.Shape;
			int dimensionIndex = shape.IndexOfFirstNonSingletonDimension();
			int sliceStep, sliceSize;
			shape.GetDimensionStepAndSize(dimensionIndex, out sliceStep, out sliceSize);

			var result = new MFullArray<double>(MArrayShape.CollapseDimension(shape, dimensionIndex));
			var resultArray = result.BackingArray;
			for (int sliceIndex = 0; sliceIndex < resultArray.Length; ++sliceIndex)
			{
				double sum = 0;
				for (int sliceElementIndex = 0; sliceElementIndex < sliceSize; ++sliceElementIndex)
					sum += array[sliceIndex * sliceSize + sliceElementIndex * sliceStep];
				resultArray[sliceIndex] = sum;
			}

			return result;
		}
		#endregion

		#region Complex Additive
		public static MFullArray<MComplex<double>> plus(MFullArray<MComplex<double>> lhs, MFullArray<MComplex<double>> rhs)
		{
			MatchShapes(ref lhs, ref rhs);
			return MFunctional.Map(lhs, rhs, plus);
		}

		public static MComplex<double> plus(MComplex<double> lhs, MComplex<double> rhs)
		{
			return new MComplex<double>(lhs.RealPart + rhs.RealPart, lhs.ImaginaryPart + rhs.ImaginaryPart);
		}
		#endregion

		#region Real Subtractive
		public static MFullArray<double> minus(MFullArray<double> lhs, MFullArray<double> rhs)
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

		public static MFullArray<double> minus(MFullArray<double> lhs, double rhs)
		{
			Contract.Requires(lhs != null);

			var c = new MFullArray<double>(lhs.Shape);
			for (int i = 0; i < lhs.Count; ++i)
				c[i] = lhs[i] - rhs;
			return c;
		}

		public static MFullArray<double> minus(double lhs, MFullArray<double> rhs)
		{
			Contract.Requires(rhs != null);

			var c = new MFullArray<double>(rhs.Shape);
			for (int i = 0; i < rhs.Count; ++i)
				c[i] = lhs - rhs[i];
			return c;
		}

		[BuiltinCilOpcode(0x59 /* sub */)]
		public static double minus(double lhs, double rhs)
		{
			return lhs - rhs;
		}

		public static MFullArray<double> uminus(MFullArray<double> array)
		{
			Contract.Requires(array != null);

			var result = new MFullArray<double>(array.Shape);
			for (int i = 0; i < array.Count; ++i)
				result[i] = -array[i];
			return result;
		}

		[BuiltinCilOpcode(0x65 /* neg */)]
		public static double uminus(double value)
		{
			return -value;
		}
		#endregion

		#region Complex Subtractive
		public static MFullArray<MComplex<double>> minus(MFullArray<MComplex<double>> lhs, MFullArray<MComplex<double>> rhs)
		{
			MatchShapes(ref lhs, ref rhs);
			return MFunctional.Map(lhs, rhs, minus);
		}

		public static MComplex<double> minus(MComplex<double> lhs, MComplex<double> rhs)
		{
			return new MComplex<double>(lhs.RealPart - rhs.RealPart, lhs.ImaginaryPart - rhs.ImaginaryPart);
		}

		public static MFullArray<MComplex<double>> uminus(MFullArray<MComplex<double>> array)
		{
			return MFunctional.Map(array, uminus);
		}

		public static MComplex<double> uminus(MComplex<double> value)
		{
			return new MComplex<double>(-value.RealPart, -value.ImaginaryPart);
		}
		#endregion

		#region Real Scalar Multiplicative
		public static MFullArray<double> times(MFullArray<double> lhs, MFullArray<double> rhs)
		{
			Contract.Requires(lhs != null);
			Contract.Requires(rhs != null);

			if (lhs.IsScalar) return times(lhs[0], rhs);
			if (rhs.IsScalar) return times(lhs, rhs[0]);
			if (lhs.Shape != rhs.Shape) throw new MArrayShapeException();

			var result = new MFullArray<double>(lhs.Shape);
			for (int i = 0; i < lhs.Count; ++i)
				result[i] = lhs[i] * rhs[i];
			return result;
		}

		public static MFullArray<double> times(MFullArray<double> lhs, double rhs)
		{
			Contract.Requires(lhs != null);

			var result = new MFullArray<double>(lhs.Shape);
			var lhsArray = lhs.BackingArray;
			var resultArray = result.BackingArray;
			int count = lhs.Count;
			for (int i = 0; i < count; ++i)
				resultArray[i] = lhsArray[i] * rhs;
			return result;
		}

		public static MFullArray<double> times(double lhs, MFullArray<double> rhs)
		{
			return times(rhs, lhs);
		}

		[BuiltinCilOpcode(0x5A /* mul */)]
		public static double times(double lhs, double rhs)
		{
			return lhs * rhs;
		}

		public static MFullArray<double> prod(MFullArray<double> array)
		{
			Contract.Requires(array != null);

			if (array.IsEmpty) return 1;

			var shape = array.Shape;
			int dimensionIndex = shape.IndexOfFirstNonSingletonDimension();
			int sliceStep, sliceSize;
			shape.GetDimensionStepAndSize(dimensionIndex, out sliceStep, out sliceSize);

			var result = new MFullArray<double>(MArrayShape.CollapseDimension(shape, dimensionIndex));
			var resultArray = result.BackingArray;
			for (int sliceIndex = 0; sliceIndex < resultArray.Length; ++sliceIndex)
			{
				double sum = 0;
				for (int sliceElementIndex = 0; sliceElementIndex < sliceSize; ++sliceElementIndex)
					sum *= array[sliceIndex * sliceSize + sliceElementIndex * sliceStep];
				resultArray[sliceIndex] = sum;
			}

			return result;
		}
		#endregion

		#region Complex Scalar Multiplicative
		public static MFullArray<MComplex<double>> times(MFullArray<MComplex<double>> lhs, MFullArray<MComplex<double>> rhs)
		{
			MatchShapes(ref lhs, ref rhs);
			return MFunctional.Map(lhs, rhs, times);
		}

		public static MComplex<double> times(MComplex<double> lhs, double rhs)
		{
			return new MComplex<double>(lhs.RealPart * rhs, lhs.ImaginaryPart * rhs);
		}

		public static MComplex<double> times(double lhs, MComplex<double> rhs)
		{
			return times(rhs, lhs);
		}

		public static MComplex<double> times(MComplex<double> lhs, MComplex<double> rhs)
		{
			return new MComplex<double>(
				lhs.RealPart * rhs.RealPart - lhs.ImaginaryPart * rhs.ImaginaryPart,
				lhs.RealPart * rhs.ImaginaryPart + lhs.ImaginaryPart * rhs.RealPart);
		}
		#endregion

		#region Scalar Divisive
		public static MFullArray<double> rdivide(MFullArray<double> lhs, MFullArray<double> rhs)
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

		public static MFullArray<double> rdivide(MFullArray<double> lhs, double rhs)
		{
			Contract.Requires(lhs != null);

			var result = new MFullArray<double>(lhs.Shape);
			for (int i = 0; i < lhs.Count; ++i)
				result[i] = lhs[i] / rhs;
			return result;
		}

		public static MFullArray<double> rdivide(double lhs, MFullArray<double> rhs)
		{
			Contract.Requires(rhs != null);

			var result = new MFullArray<double>(rhs.Shape);
			for (int i = 0; i < rhs.Count; ++i)
				result[i] = lhs / rhs[i];
			return result;
		}

		[BuiltinCilOpcode(0x5B /* div */)]
		public static double rdivide(double lhs, double rhs)
		{
			return lhs / rhs;
		}

		public static MFullArray<double> ldivide(MFullArray<double> lhs, MFullArray<double> rhs)
		{
			Contract.Requires(lhs != null);
			Contract.Requires(rhs != null);

			return rdivide(rhs, lhs);
		}

		// NOT equivalent to 'div' opcode
		public static double ldivide(double lhs, double rhs)
		{
			return rhs / lhs;
		}

		public static MFullArray<double> mod(MFullArray<double> lhs, MFullArray<double> rhs)
		{
			Contract.Requires(lhs != null);
			Contract.Requires(rhs != null);

			MatchShapes(ref lhs, ref rhs);
			return MFunctional.Map(lhs, rhs, mod);
		}

		public static double mod(double lhs, double rhs)
		{
			return lhs - times(floor(rdivide(lhs, rhs)), rhs);
		}

		public static MFullArray<double> rem(MFullArray<double> lhs, MFullArray<double> rhs)
		{
			Contract.Requires(lhs != null);
			Contract.Requires(rhs != null);

			MatchShapes(ref lhs, ref rhs);
			return MFunctional.Map(lhs, rhs, rem);
		}

		public static double rem(double lhs, double rhs)
		{
			return lhs - times(fix(rdivide(lhs, rhs)), rhs);
		}
		#endregion

		#region Real Matrix Operations
		public static MFullArray<double> mtimes(MFullArray<double> lhs, MFullArray<double> rhs)
		{
			Contract.Requires(lhs != null);
			Contract.Requires(rhs != null);

			var rhsShape = rhs.Shape;
			if (rhsShape.IsScalar) return times(lhs, rhs[0]);

			var lhsShape = lhs.Shape;
			if (lhsShape.IsScalar) return times(rhs, lhs[0]);

			if (lhsShape.IsHigherDimensional || rhs.IsHigherDimensional)
				throw new MArrayShapeException("Cannot multiply higher-dimensional matrices.");

			if (lhsShape.ColumnCount != rhsShape.RowCount)
				throw new MArrayShapeException("Matrix shape mismatch for multiplication.");

			var result = new MFullArray<double>(lhsShape.RowCount, rhsShape.ColumnCount);
			mtimes_core(lhsShape.RowCount, lhsShape.ColumnCount, rhsShape.ColumnCount,
				lhs.BackingArray, rhs.BackingArray, result.BackingArray);

			return result;
		}

		private static void mtimes_core(int m, int n, int p, double[] lhs, double[] rhs, double[] result)
		{
			// m×n * n×p = m×p
			int index = 0;
			for (int j = 0; j < p; ++j)
			{
				int rhsColumnOffset = j * n;
				for (int i = 0; i < m; ++i)
				{
					double value = 0;
					for (int k = 0; k < n; ++k)
						value += lhs[i + k * m] * rhs[k + rhsColumnOffset];
					result[index] = value;
					index++;
				}
			}
		}

		public static MFullArray<double> mtimes(MFullArray<double> lhs, double rhs)
		{
			return times(lhs, rhs);
		}

		public static MFullArray<double> mtimes(double lhs, MFullArray<double> rhs)
		{
			return times(lhs, rhs);
		}

		[BuiltinCilOpcode(0x5A /* mul */)]
		public static double mtimes(double lhs, double rhs)
		{
			return lhs * rhs;
		}

		public static MFullArray<double> mpower(MFullArray<double> @base, double exponent)
		{
			Contract.Requires(@base != null);

			if (!@base.IsSquareMatrix) throw new MArrayShapeException();
			if (@base.IsScalar) return mpower(@base[0], exponent);

			throw new NotImplementedException("Non-scalar mpower.");
		}

		public static double mpower(double @base, double exponent)
		{
			return power(@base, exponent);
		}

		public static MFullArray<double> mrdivide(MFullArray<double> b, MFullArray<double> a)
		{
			Contract.Requires(b != null);
			Contract.Requires(a != null);

			if (a.IsScalar) return mrdivide(b, a[0]);
			if (a.IsHigherDimensional || b.IsHigherDimensional || a.ColumnCount != b.ColumnCount)
				throw new MArrayShapeException();

			if (a.IsScalar && b.IsScalar) return mrdivide(b[0], a[0]);

			throw new NotImplementedException("Non-scalar mrdivide.");
		}

		public static MFullArray<double> mrdivide(MFullArray<double> b, double a)
		{
			return rdivide(b, a);
		}

		[BuiltinCilOpcode(0x5B /* div */)]
		public static double mrdivide(double b, double a)
		{
			return b / a;
		}

		public static MFullArray<double> mldivide(MFullArray<double> a, MFullArray<double> b)
		{
			Contract.Requires(b != null);
			Contract.Requires(a != null);

			if (a.IsHigherDimensional || b.IsHigherDimensional || a.RowCount != b.RowCount)
				throw new MArrayShapeException();

			if (a.IsScalar && b.IsScalar) return mldivide(a[0], b[0]);

			throw new NotImplementedException("Non-scalar mldivide.");
		}

		[BuiltinCilOpcode(0x5B /* div */)]
		public static double mldivide(double a, double b)
		{
			return b / a;
		}
		#endregion

		#region Complex Matrix Operations
		public static MComplex<double> mtimes(MComplex<double> lhs, double rhs)
		{
			return times(lhs, rhs);
		}

		public static MComplex<double> mtimes(double lhs, MComplex<double> rhs)
		{
			return times(lhs, rhs);
		}

		public static MComplex<double> mtimes(MComplex<double> lhs, MComplex<double> rhs)
		{
			return times(lhs, rhs);
		}
		#endregion

		#region Power
		public static MFullArray<double> power(MFullArray<double> @base, MFullArray<double> exponent)
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

		public static MFullArray<double> power(MFullArray<double> @base, double exponent)
		{
			Contract.Requires(@base != null);

			var result = new MFullArray<double>(@base.Shape);
			for (int i = 0; i < @base.Count; ++i)
				result[i] = power(@base[i], exponent);
			return result;
		}

		public static MFullArray<double> power(double @base, MFullArray<double> exponent)
		{
			Contract.Requires(exponent != null);

			var result = new MFullArray<double>(exponent.Shape);
			for (int i = 0; i < exponent.Count; ++i)
				result[i] = power(@base, exponent[i]);
			return result;
		}

		public static double power(double @base, double exponent)
		{
			return Math.Pow(@base, exponent);
		}
		#endregion

		#region Rounding
		public static MFullArray<double> floor(MFullArray<double> array)
		{
			Contract.Requires(array != null);

			var result = new MFullArray<double>(array.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = floor(array[i]);
			return result;
		}

		public static double floor(double value)
		{
			return Math.Floor(value);
		}

		public static MFullArray<double> ceil(MFullArray<double> array)
		{
			Contract.Requires(array != null);

			var result = new MFullArray<double>(array.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = ceil(array[i]);
			return result;
		}

		public static double ceil(double value)
		{
			return Math.Ceiling(value);
		}

		public static MFullArray<double> round(MFullArray<double> array)
		{
			Contract.Requires(array != null);

			var result = new MFullArray<double>(array.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = round(array[i]);
			return result;
		}

		public static double round(double value)
		{
			// Unfortunately, portable class libraries do not have
			// Math.Round(..., MidpointRounding, AwayFromZero).

			if (value > 0)
				return (value % 1) == 0.5 ? ceil(value) : floor(value);
			else
				return (value % 1) == -0.5 ? floor(value) : ceil(value);
		}

		public static MFullArray<double> fix(MFullArray<double> array)
		{
			Contract.Requires(array != null);

			var result = new MFullArray<double>(array.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = fix(array[i]);
			return result;
		}

		public static double fix(double value)
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
		internal static void MatchShapes<TScalar>(ref MFullArray<TScalar> first, ref MFullArray<TScalar> second)
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
