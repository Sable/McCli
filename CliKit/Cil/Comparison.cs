using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CliKit.Cil
{
	[Serializable]
	public enum Comparison
	{
		Equal,
		NotEqual,
		
		GreaterThanOrEqual,
		GreaterThan,
		LessThanOrEqual,
		LessThan,

		GreaterThanOrEqual_Unsigned,
		GreaterThanOrEqual_Unordered = GreaterThanOrEqual_Unsigned,
		GreaterThan_Unsigned,
		GreaterThan_Ordered = GreaterThan_Unsigned,
		LessThanOrEqual_Unsigned,
		LessThanOrEqual_Ordered = LessThanOrEqual_Unsigned,
		LessThan_Unsigned,
		LessThan_Unordered = LessThan_Unsigned,
	}

	public static class ComparisonExtensions
	{
		public static bool HasCompareOpcode(this Comparison comparison)
		{
			return comparison == Comparison.Equal
				|| comparison == Comparison.GreaterThan
				|| comparison == Comparison.LessThan
				|| comparison == Comparison.GreaterThan_Unsigned
				|| comparison == Comparison.LessThan_Unsigned;
		}

		public static bool IsUnsignedOrUnordered(this Comparison comparison)
		{
			return comparison >= Comparison.GreaterThan_Unsigned;
		}
	}
}
