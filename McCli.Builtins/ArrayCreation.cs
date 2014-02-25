using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Builtins
{
	/// <summary>
	/// Implements MatLab builtins for array creation and concatenation.
	/// http://www.mathworks.com/help/matlab/elementary-matrices-and-arrays.html
	/// </summary>
	[MatlabLibrary]
	public static class ArrayCreation
	{
		// TODO: numerical coercion for those (can call zeros(int16(4)))

		#region Zeros
		public static double zeros()
		{
			return 0;
		}

		public static MFullArray<double> zeros(double n)
		{
			return zeros(n, n);
		}

		public static MFullArray<double> zeros(double sz1, double sz2)
		{
			return new MFullArray<double>(Utilities.ToShape(sz1, sz2));
		}
		#endregion

		#region Ones
		public static double ones()
		{
			return 1;
		}

		public static MFullArray<double> ones(double n)
		{
			return ones(n, n);
		}

		public static MFullArray<double> ones(double sz1, double sz2)
		{
			return MFullArray<double>.ExpandScalar(1, Utilities.ToShape(sz1, sz2));
		}
		#endregion

		#region Eye
		public static double eye()
		{
			return 1;
		}

		public static MFullArray<double> eye(double n)
		{
			return eye(n, n);
		}

		public static MFullArray<double> eye(double sz1, double sz2)
		{
			var shape = Utilities.ToShape(sz1, sz2);
			var result = new MFullArray<double>(shape);
			var array = result.BackingArray;
			for (int i = 0; i < array.Length; ++i)
				array[i] = (i / shape.RowCount) == (i % shape.RowCount) ? 1 : 0;
			return result;
		}
		#endregion

		#region True
		public static bool @true() { return true; }

		public static MFullArray<bool> @true(double n)
		{
			return @true(n, n);
		}

		public static MFullArray<bool> @true(double sz1, double sz2)
		{
			return MFullArray<bool>.ExpandScalar(true, Utilities.ToShape(sz1, sz2));
		}
		#endregion

		#region False
		public static bool @false() { return false; }

		public static MFullArray<bool> @false(double n)
		{
			return @false(n, n);
		}

		public static MFullArray<bool> @false(double sz1, double sz2)
		{
			return new MFullArray<bool>(Utilities.ToShape(sz1, sz2));
		}
		#endregion

		public static MArray<TScalar> arrayfun<TScalar>(Func<TScalar, TScalar> map, MArray<TScalar> array)
		{
			Contract.Requires(map != null);
			Contract.Requires(array != null);

			var result = new MFullArray<TScalar>(array.Shape);
			for (int i = 0; i < array.Count; ++i)
				result[i] = map(array[i]);
			return result;
		}

		public static MArray<TScalar> arrayfun<TScalar>(Func<TScalar, TScalar, TScalar> map, MArray<TScalar> array1, MArray<TScalar> array2)
		{
			Contract.Requires(map != null);
			Contract.Requires(array1 != null);
			Contract.Requires(array2 != null);

			var shape = array1.Shape;
			if (array2.Shape != array1.Shape) throw new MArrayShapeException();

			var result = new MFullArray<TScalar>(shape);
			for (int i = 0; i < shape.Count; ++i)
				result[i] = map(array1[i], array2[i]);
			return result;
		}

		// TODO: diag, pascal, magic
	}
}
