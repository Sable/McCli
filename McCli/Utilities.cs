using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	public static class Utilities
	{
		public static bool IsIntegralFloat(double value)
		{
			return Math.Floor(value) == value;
		}

		public static int IntCoerce(double value)
		{
			var i = (int)value;
			if (i != value) throw new InvalidCastException();
			return i;
		}

		public static int IntCoerce(float value)
		{
			var i = (int)value;
			if (i != value) throw new InvalidCastException();
			return i;
		}

		public static T AsScalar<T>(MArray<T> array)
		{
			Contract.Requires(array != null);

			if (!array.IsScalar) throw new InvalidCastException();
			return array.BackingArray[0];
		}

		public static bool IsTrue(MValue value)
		{
			// An evaluated expression is true when the result is nonempty
			// and contains all nonzero elements (logical or real numeric).
			// Otherwise, the expression is false.
			Contract.Requires(value != null);

			var type = value.MType;
			if (type.IsComplex) return false;

			var array = value as MArray;
			if (array != null)
			{
				int count = array.ElementCount;
				if (count == 0) return false;

				var doubleArray = array as MArray<double>;
				if (doubleArray != null)
				{
					for (int i = 0; i < count; ++i)
						if (doubleArray[i] != 0)
							return false;
					return true;
				}

				var logicalArray = array as MArray<bool>;
				if (logicalArray != null)
				{
					for (int i = 0; i < count; ++i)
						if (!logicalArray[i])
							return false;
					return true;
				}
			}

			throw new NotImplementedException();
		}
	}
}
