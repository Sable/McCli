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
	}
}
