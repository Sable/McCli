using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace McCli.Builtins
{
	/// <summary>
	/// Implements MatLab builtins that relate to array dimensions.
	/// http://www.mathworks.com/help/matlab/basic-information.html
	/// </summary>
	[MatlabLibrary]
	public static class ArrayDimensions
	{
		#region Counting
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
				result[i] = shape.GetDimensionSize(i);
			return result;
		}

		public static double size(MValue value, double dim)
		{
			Contract.Requires(value != null);
			var shape = value.Shape;
			return shape.GetDimensionSize(PseudoBuiltins.ToInt(dim) - 1);
		}

		public static double length(MValue value)
		{
			Contract.Requires(value != null);

			var shape = value.Shape;
			int result = 0;
			for (int i = 0; i < shape.DimensionCount; ++i)
				result = Math.Max(result, shape.GetDimensionSize(i));
			return result;
		}
		#endregion

		#region Testing
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
	}
}
