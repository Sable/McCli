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
		private readonly int first, count;
		#endregion

		#region Constructors
		private MIntegralRange(int first, int count)
		{
			this.first = first;
			this.count = count;
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

		public int End
		{
			get { return first + count; }
		}

		public bool IsEmpty
		{
			get { return count <= 0; }
		}

		public int Count
		{
			get { return count; }
		}
		#endregion

		#region Methods
		public bool Equals(MIntegralRange<TReal> other)
		{
			return first == other.first && count == other.count;
		}

		public override bool Equals(object obj)
		{
			return obj is MIntegralRange<TReal> && Equals((MIntegralRange<TReal>)obj);
		}

		public override int GetHashCode()
		{
			return first ^ (count << 13);
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", first, first + count - 1);
		}

		public static MIntegralRange<TReal> FromFirstCount(int first, int count)
		{
			Contract.Requires(count >= 0);
			return new MIntegralRange<TReal>(first, count);
		}
		#endregion
	}
}
