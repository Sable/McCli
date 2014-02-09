﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// Provides implementation of some of MatLab's built-in functions.
	/// </summary>
	public static class Builtins
	{
		#region Classes
		public static string @class(MValue value)
		{
			Contract.Requires(value != null);
			return value.Class.Name;
		}

		public static bool isa(MValue value, MFullArray<char> className)
		{
			Contract.Requires(value != null);
			Contract.Requires(className != null);

			var @class = value.Class;
			var classNameString = Utilities.AsString(className);
			if (@class.Name == classNameString) return true;

			// "categories"
			var classKind = @class.Kind;
			if (classNameString == "numeric" && (classKind & MClassKinds.NumericMask) != 0) return true;
			if (classNameString == "float" && (classKind & MClassKinds.FloatMask) != 0) return true;
			if (classNameString == "integer" && (classKind & MClassKinds.IntegerMask) != 0) return true;
			return false;
		}

		public static bool iscell(MValue value)
		{
			Contract.Requires(value != null);
			return value.Class == MClass.Cell;
		}

		public static bool ischar(MValue value)
		{
			Contract.Requires(value != null);
			return value.Class == MClass.Char;
		}

		public static bool islogical(MValue value)
		{
			Contract.Requires(value != null);
			return value.Class == MClass.Logical;
		}

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

		public static bool ischar(char value) { return true; }
		public static bool islogical(bool value) { return true; }

		// TODO: is*
		#endregion

		#region Execution Environment
		public static bool ispc()
		{
			var platform = Environment.OSVersion.Platform;
			return platform == PlatformID.Win32NT
				|| platform == PlatformID.Win32Windows
				|| platform == PlatformID.Win32S;
		}

		public static bool isunix()
		{
			return Environment.OSVersion.Platform == PlatformID.Unix;
		}

		public static bool ismac()
		{
			return Environment.OSVersion.Platform == PlatformID.MacOSX;
		}

		public static bool isstudent()
		{
			return false; // !
		}
		#endregion

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

		#region Floating Point Stuff
		#region Rounding
		public static MArray<double> floor(MArray<double> array)
		{
			Contract.Requires(array != null);

			var result = new MFullArray<double>(array.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = Math.Floor(array[i]);
			return result;
		}

		public static MArray<double> ceil(MArray<double> array)
		{
			Contract.Requires(array != null);

			var result = new MFullArray<double>(array.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = Math.Ceiling(array[i]);
			return result;
		}

		public static MArray<double> round(MArray<double> array)
		{
			Contract.Requires(array != null);

			var result = new MFullArray<double>(array.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = Math.Round(array[i], MidpointRounding.AwayFromZero);
			return result;
		}

		public static MArray<double> fix(MArray<double> array)
		{
			Contract.Requires(array != null);

			var result = new MFullArray<double>(array.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = Math.Truncate(array[i]);
			return result;
		}
		#endregion

		#region Finite/Infinity/NaN testing
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

		#region Array Creation
		// TODO: numerical coercion for those (can call zeros(int16(4)))

		#region Zeroes
		public static double zeros()
		{
			return 0;
		}

		public static MFullArray<double> zeros(int n)
		{
			Contract.Requires(n >= 0);
			return zeros(n, n);
		}

		public static MFullArray<double> zeros(int sz1, int sz2)
		{
			Contract.Requires(sz1 >= 0);
			Contract.Requires(sz2 >= 0);
			return new MFullArray<double>(sz1, sz2);
		}
		#endregion

		#region Ones
		public static double ones()
		{
			return 1;
		}

		public static MFullArray<double> ones(int n)
		{
			Contract.Requires(n >= 0);
			return ones(n, n);
		}

		public static MFullArray<double> ones(int sz1, int sz2)
		{
			Contract.Requires(sz1 >= 0);
			Contract.Requires(sz2 >= 0);
			return MFullArray<double>.ExpandScalar(1, new MArrayShape(sz1, sz2));
		}
		#endregion

		#region Eye
		public static double eye()
		{
			return 1;
		}

		public static MFullArray<double> eye(int n)
		{
			Contract.Requires(n >= 0);
			return eye(n, n);
		}

		public static MFullArray<double> eye(int sz1, int sz2)
		{
			Contract.Requires(sz1 >= 0);
			Contract.Requires(sz2 >= 0);

			var result = new MFullArray<double>(sz1, sz2);
			var array = result.BackingArray;
			for (int i = 0; i < array.Length; ++i)
				array[i] = (i / sz1) == (i % sz1) ? 1 : 0;
			return result;
		}
		#endregion

		#region True/False
		public static bool @true() { return true; }

		public static bool @false() { return false; }

		// TODO: Sized true/false
		#endregion

		// TODO: diag
		#endregion

		#region Array Shape
		#region Size & Counting
		public static double numel(MValue value)
		{
			Contract.Requires(value != null);
			return value.Count;
		}

		public static double ndims(MValue value)
		{
			Contract.Requires(value != null);
			return value.Shape.DimensionCount;
		}

		public static MFullArray<double> size(MValue value)
		{
			Contract.Requires(value != null);

			var shape = value.Shape;
			var result = new MFullArray<double>(MArrayShape.RowVector(shape.DimensionCount));
			for (int i = 0; i < shape.DimensionCount; ++i)
				result[i] = shape.GetSize(i);
			return result;
		}

		public static double length(MValue value)
		{
			Contract.Requires(value != null);

			var shape = value.Shape;
			int result = 0;
			for (int i = 0; i < shape.DimensionCount; ++i)
				result = Math.Max(result, shape.GetSize(i));
			return result;
		}
		#endregion

		#region Shape Testing
		public static bool isempty(MValue value)
		{
			Contract.Requires(value != null);
			return value.IsEmpty;
		}

		public static bool isscalar(MValue value)
		{
			Contract.Requires(value != null);
			return value.IsScalar;
		}

		public static bool iscolumn(MValue value)
		{
			Contract.Requires(value != null);
			return value.IsColumnVector;
		}

		public static bool isrow(MValue value)
		{
			Contract.Requires(value != null);
			return value.IsRowVector;
		}

		public static bool isvector(MValue value)
		{
			Contract.Requires(value != null);
			return value.Shape.IsVector;
		}

		public static bool ismatrix(MValue value)
		{
			Contract.Requires(value != null);
			return !value.IsHigherDimensional;
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

			var result = new MFullArray<TReal>(array.Shape);
			for (int i = 0; i < array.Count; ++i)
				result[i] = array[i].RealPart;
			return result;
		}

		public static MArray<TReal> imag<[AnyRealNumeric] TReal>(MArray<MComplex<TReal>> array) where TReal : struct
		{
			Contract.Requires(array != null);

			var result = new MFullArray<TReal>(array.Shape);
			for (int i = 0; i < array.Count; ++i)
				result[i] = array[i].ImaginaryPart;
			return result;
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
