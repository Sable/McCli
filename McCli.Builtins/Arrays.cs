using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Builtins
{
	/// <summary>
	/// Implements array-related MatLab builtins.
	/// </summary>
	[MatlabLibrary]
	public static class Arrays
	{
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
	}
}
