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
		public static MArray<double> plus(MArray<double> lhs, MArray<double> rhs)
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

		public static MArray<double> plus(MArray<double> lhs, double rhs)
		{
			Contract.Requires(lhs != null);

			var c = new MFullArray<double>(lhs.Shape);
			for (int i = 0; i < lhs.Count; ++i)
				c[i] = lhs[i] + rhs;
			return c;
		}

		public static MArray<double> plus(double lhs, MArray<double> rhs)
		{
			return plus(rhs, lhs);
		}

		[BuiltinCilOpcode(0x58 /* add */)]
		public static double plus(double lhs, double rhs)
		{
			return lhs + rhs;
		}

		public static MArray<TScalar> uplus<[AnyNumeric]TScalar>(MArray<TScalar> array)
		{
			Contract.Requires(array != null);
			return array.DeepClone();
		}

		public static TScalar uplus<[AnyNumeric]TScalar>(TScalar value)
		{
			return value;
		}

		public static MArray<double> sum(MArray<double> array)
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
		public static MArray<MComplex<double>> plus(MArray<MComplex<double>> lhs, MArray<MComplex<double>> rhs)
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
		public static MArray<double> minus(MArray<double> lhs, MArray<double> rhs)
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

		public static MArray<double> minus(MArray<double> lhs, double rhs)
		{
			Contract.Requires(lhs != null);

			var c = new MFullArray<double>(lhs.Shape);
			for (int i = 0; i < lhs.Count; ++i)
				c[i] = lhs[i] - rhs;
			return c;
		}

		public static MArray<double> minus(double lhs, MArray<double> rhs)
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

		public static MArray<double> uminus(MArray<double> array)
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
		public static MArray<MComplex<double>> minus(MArray<MComplex<double>> lhs, MArray<MComplex<double>> rhs)
		{
			MatchShapes(ref lhs, ref rhs);
			return MFunctional.Map(lhs, rhs, minus);
		}

		public static MComplex<double> minus(MComplex<double> lhs, MComplex<double> rhs)
		{
			return new MComplex<double>(lhs.RealPart - rhs.RealPart, lhs.ImaginaryPart - rhs.ImaginaryPart);
		}

		public static MArray<MComplex<double>> uminus(MArray<MComplex<double>> array)
		{
			return MFunctional.Map(array, uminus);
		}

		public static MComplex<double> uminus(MComplex<double> value)
		{
			return new MComplex<double>(-value.RealPart, -value.ImaginaryPart);
		}
		#endregion

		#region Real Scalar Multiplicative
		public static MArray<double> times(MArray<double> lhs, MArray<double> rhs)
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

		public static MArray<double> times(MArray<double> lhs, double rhs)
		{
			Contract.Requires(lhs != null);

			var result = new MFullArray<double>(lhs.Shape);
			for (int i = 0; i < lhs.Count; ++i)
				result[i] = lhs[i] * rhs;
			return result;
		}

		public static MArray<double> times(double lhs, MArray<double> rhs)
		{
			return times(rhs, lhs);
		}

		[BuiltinCilOpcode(0x5A /* mul */)]
		public static double times(double lhs, double rhs)
		{
			return lhs * rhs;
		}

		public static MArray<double> prod(MArray<double> array)
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
		public static MArray<MComplex<double>> times(MArray<MComplex<double>> lhs, MArray<MComplex<double>> rhs)
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
		public static MArray<double> rdivide(MArray<double> lhs, MArray<double> rhs)
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

		public static MArray<double> rdivide(MArray<double> lhs, double rhs)
		{
			Contract.Requires(lhs != null);

			var result = new MFullArray<double>(lhs.Shape);
			for (int i = 0; i < lhs.Count; ++i)
				result[i] = lhs[i] / rhs;
			return result;
		}

		public static MArray<double> rdivide(double lhs, MArray<double> rhs)
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

		public static MArray<double> ldivide(MArray<double> lhs, MArray<double> rhs)
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

		public static MArray<double> mod(MArray<double> lhs, MArray<double> rhs)
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

		public static MArray<double> rem(MArray<double> lhs, MArray<double> rhs)
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
		public static MArray<double> mtimes(MArray<double> lhs, MArray<double> rhs)
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

		public static MArray<double> mtimes(MArray<double> lhs, double rhs)
		{
			return times(lhs, rhs);
		}

		public static MArray<double> mtimes(double lhs, MArray<double> rhs)
		{
			return times(lhs, rhs);
		}

		[BuiltinCilOpcode(0x5A /* mul */)]
		public static double mtimes(double lhs, double rhs)
		{
			return lhs * rhs;
		}

		public static MArray<double> mpower(MArray<double> @base, double exponent)
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

		public static MArray<double> mrdivide(MArray<double> b, MArray<double> a)
		{
			Contract.Requires(b != null);
			Contract.Requires(a != null);

			if (a.IsScalar) return mrdivide(b, a[0]);
			if (a.IsHigherDimensional || b.IsHigherDimensional || a.ColumnCount != b.ColumnCount)
				throw new MArrayShapeException();

			if (a.IsScalar && b.IsScalar) return mrdivide(b[0], a[0]);

			throw new NotImplementedException("Non-scalar mrdivide.");
		}

		public static MArray<double> mrdivide(MArray<double> b, double a)
		{
			return rdivide(b, a);
		}

		[BuiltinCilOpcode(0x5B /* div */)]
		public static double mrdivide(double b, double a)
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
		public static MArray<double> power(MArray<double> @base, MArray<double> exponent)
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

		public static MArray<double> power(MArray<double> @base, double exponent)
		{
			Contract.Requires(@base != null);

			var result = new MFullArray<double>(@base.Shape);
			for (int i = 0; i < @base.Count; ++i)
				result[i] = power(@base[i], exponent);
			return result;
		}

		public static MArray<double> power(double @base, MArray<double> exponent)
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
		public static MArray<double> floor(MArray<double> array)
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

		public static MArray<double> ceil(MArray<double> array)
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

		public static MArray<double> round(MArray<double> array)
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

		public static MArray<double> fix(MArray<double> array)
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
