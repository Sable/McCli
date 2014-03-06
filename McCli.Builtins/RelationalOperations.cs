using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace McCli.Builtins
{
	/// <summary>
	/// Implements MatLab builtins for relational operations.
	/// http://www.mathworks.com/help/matlab/relational-operators.html
	/// </summary>
	[MatlabLibrary]
	public static class RelationalOperations
	{
		#region Comparison
		#region Double Arrays
		public static MArray<bool> eq(MArray<double> a, MArray<double> b)
		{
			ElementaryOperations.MatchShapes(ref a, ref b);

			var result = new MFullArray<bool>(a.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = a[i] == b[i];
			return result;
		}

		public static MArray<bool> ne(MArray<double> a, MArray<double> b)
		{
			ElementaryOperations.MatchShapes(ref a, ref b);

			var result = new MFullArray<bool>(a.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = a[i] != b[i];
			return result;
		}

		public static MArray<bool> gt(MArray<double> a, MArray<double> b)
		{
			ElementaryOperations.MatchShapes(ref a, ref b);

			var result = new MFullArray<bool>(a.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = a[i] > b[i];
			return result;
		}

		public static MArray<bool> lt(MArray<double> a, MArray<double> b)
		{
			ElementaryOperations.MatchShapes(ref a, ref b);

			var result = new MFullArray<bool>(a.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = a[i] < b[i];
			return result;
		}

		public static MArray<bool> ge(MArray<double> a, MArray<double> b)
		{
			ElementaryOperations.MatchShapes(ref a, ref b);

			var result = new MFullArray<bool>(a.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = a[i] >= b[i];
			return result;
		}

		public static MArray<bool> le(MArray<double> a, MArray<double> b)
		{
			ElementaryOperations.MatchShapes(ref a, ref b);

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

		#region Double Scalars
		public static bool eq(double a, double b) { return a == b; }
		public static bool ne(double a, double b) { return a != b; }
		public static bool ge(double a, double b) { return a >= b; }
		public static bool gt(double a, double b) { return a > b; }
		public static bool le(double a, double b) { return a <= b; }
		public static bool lt(double a, double b) { return a < b; }
		public static bool isequal(double a, double b) { return a == b; }
		#endregion

		#region Char
		public static MArray<bool> eq(MArray<char> a, MArray<char> b)
		{
			return MFunctional.Zip(a, b, (x, y) => x == y, allowScalarArgument: true);
		}

		public static MArray<bool> ne(MArray<char> a, MArray<char> b)
		{
			return MFunctional.Zip(a, b, (x, y) => x != y, allowScalarArgument: true);
		}

		public static bool isequal(MArray<char> a, MArray<char> b)
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

		// TODO: isequaln
		#endregion
	}
}
