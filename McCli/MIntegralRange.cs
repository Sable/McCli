using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;

namespace McCli
{
	/// <summary>
	/// Represents a contiguous range of values of some Matlab real type,
	/// as obtained from a <c>i1:i2</c> expression.
	/// </summary>
	/// <typeparam name="TReal">The underlying Matlab type.</typeparam>
	public struct MIntegralRange<TReal> : IEquatable<MIntegralRange<TReal>>
	{
		#region Fields
		private readonly int first, last;
		#endregion

		#region Constructors
		private MIntegralRange(int first, int last)
		{
			this.first = first;
			this.last = last;
		}

		static MIntegralRange()
		{
			var @class = MType.FromCliType(typeof(TReal));
			Contract.Assert(@class != null && @class.IsNumeric && !@class.IsComplex);
		}
		#endregion

		#region Properties
		public int First
		{
			get { return first; }
		}

		public int Last
		{
			get { return last; }
		}

		public bool IsEmpty
		{
			get { return last < first; }
		}

		public int Count
		{
			get { return Math.Max(last - first + 1, 0); }
		}
		#endregion

		#region Methods
		public bool Equals(MIntegralRange<TReal> other)
		{
			return first == other.first && last == other.last;
		}

		public override bool Equals(object obj)
		{
			return obj is MIntegralRange<TReal> && Equals((MIntegralRange<TReal>)obj);
		}

		public override int GetHashCode()
		{
			return first ^ (last << 13);
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", first, last);
		}

		public static MIntegralRange<TReal> FromValue(int value)
		{
			return new MIntegralRange<TReal>(value, value);
		}

		public static MIntegralRange<TReal> FromFirstLast(int first, int last)
		{
			return new MIntegralRange<TReal>(first, last);
		}

		public static MIntegralRange<TReal> FromFirstCount(int first, int count)
		{
			Contract.Requires(count >= 0);
			return new MIntegralRange<TReal>(first, first + count - 1);
		}
		#endregion
	}
}
