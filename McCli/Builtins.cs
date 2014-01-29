using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	public static class Builtins
	{
		#region Arithmetic operators
		public static MArray<double> plus(MArray<double> a, MArray<double> b)
		{
			Contract.Requires(a != null);
			Contract.Requires(b != null);
			Contract.Requires(a.Shape == b.Shape);

			var c = new MArray<double>(a.Shape);
			var aArray = a.BackingArray;
			for (int i = 0; i < aArray.Length; ++i)
				c[i] = a[i] + b[i];
			return c;
		}

		public static MArray<double> plus(MArray<double> a, double b)
		{
			Contract.Requires(a != null);

			var c = new MArray<double>(a.Shape);
			for (int i = 0; i < a.ElementCount; ++i)
				c[i] = a[i] + b;
			return c;
		}

		public static MArray<double> minus(MArray<double> a, MArray<double> b)
		{
			Contract.Requires(a != null);
			Contract.Requires(b != null);
			Contract.Requires(a.Shape == b.Shape);

			var c = new MArray<double>(a.Shape);
			var aArray = a.BackingArray;
			for (int i = 0; i < aArray.Length; ++i)
				c[i] = a[i] - b[i];
			return c;
		}

		public static MArray<double> minus(MArray<double> a, double b)
		{
			Contract.Requires(a != null);

			var c = new MArray<double>(a.Shape);
			for (int i = 0; i < a.ElementCount; ++i)
				c[i] = a[i] - b;
			return c;
		}
		#endregion

		#region Comparison
		public static bool isequal(MArray<double> a, MArray<double> b)
		{
			Contract.Requires(a != null);
			Contract.Requires(b != null);

			if (a.Shape != b.Shape) return false;

			var aArray = a.BackingArray;
			var bArray = b.BackingArray;
			int count = a.ElementCount;

			for (int i = 0; i < count; ++i)
				if (aArray[i] != bArray[i])
					return false;

			return true;
		}
		#endregion

		#region Colon
		public static MArray<double> colon(double low, double high)
		{
			if (low > high) return new MArray<double>(1, 0);

			var count = (int)(high - low) + 1;
			var array = new MArray<double>(1, count);
			for (int i = 0; i < count; ++i)
				array[i] = low + i;

			return array;
		}
		#endregion
	}
}
