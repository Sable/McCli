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
		public static MFullArray<bool> eq(MFullArray<double> a, MFullArray<double> b)
		{
			ElementaryOperations.MatchShapes(ref a, ref b);

			var result = new MFullArray<bool>(a.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = a[i] == b[i];
			return result;
		}

		public static MFullArray<bool> ne(MFullArray<double> a, MFullArray<double> b)
		{
			ElementaryOperations.MatchShapes(ref a, ref b);

			var result = new MFullArray<bool>(a.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = a[i] != b[i];
			return result;
		}

		public static MFullArray<bool> gt(MFullArray<double> a, MFullArray<double> b)
		{
			ElementaryOperations.MatchShapes(ref a, ref b);

			var result = new MFullArray<bool>(a.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = a[i] > b[i];
			return result;
		}

		public static MFullArray<bool> lt(MFullArray<double> a, MFullArray<double> b)
		{
			ElementaryOperations.MatchShapes(ref a, ref b);

			var result = new MFullArray<bool>(a.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = a[i] < b[i];
			return result;
		}

		public static MFullArray<bool> ge(MFullArray<double> a, MFullArray<double> b)
		{
			ElementaryOperations.MatchShapes(ref a, ref b);

			var result = new MFullArray<bool>(a.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = a[i] >= b[i];
			return result;
		}

		public static MFullArray<bool> le(MFullArray<double> a, MFullArray<double> b)
		{
			ElementaryOperations.MatchShapes(ref a, ref b);

			var result = new MFullArray<bool>(a.Shape);
			for (int i = 0; i < result.Count; ++i)
				result[i] = a[i] <= b[i];
			return result;
		}

		public static bool isequal(MFullArray<double> a, MFullArray<double> b)
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
		[BuiltinCilOpcode(0xFE01 /* ceq */)]
		public static bool eq(double a, double b) { return a == b; }
		[BuiltinCilOpcode(0xFE01, 0x16, 0xFE01 /* ceq, ldc.i4.0, ceq */)]
		public static bool ne(double a, double b) { return a != b; }
		[BuiltinCilOpcode(0xFE04, 0x16, 0xFE01 /* clt, ldc.i4.0, ceq */)]
		public static bool ge(double a, double b) { return a >= b; }
		[BuiltinCilOpcode(0xFE02 /* cgt */)]
		public static bool gt(double a, double b) { return a > b; }
		[BuiltinCilOpcode(0xFE02, 0x16, 0xFE01 /* cgt, ldc.i4.0, ceq */)]
		public static bool le(double a, double b) { return a <= b; }
		[BuiltinCilOpcode(0xFE04 /* clt */)]
		public static bool lt(double a, double b) { return a < b; }
		[BuiltinCilOpcode(0xFE01 /* ceq */)]
		public static bool isequal(double a, double b) { return a == b; }
		#endregion

		#region Char
		public static MFullArray<bool> eq(MFullArray<char> a, MFullArray<char> b)
		{
			return MFunctional.Zip(a, b, (x, y) => x == y, allowScalarArgument: true);
		}

		public static MFullArray<bool> ne(MFullArray<char> a, MFullArray<char> b)
		{
			return MFunctional.Zip(a, b, (x, y) => x != y, allowScalarArgument: true);
		}

		public static bool isequal(MFullArray<char> a, MFullArray<char> b)
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
