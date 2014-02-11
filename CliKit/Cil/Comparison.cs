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

		OperationMask = 0xF,

		UnsignedFlag = 0x10,
		UnorderedFlag = UnsignedFlag,

		NotEqual_Unsigned = NotEqual | UnsignedFlag,
		NotEqual_Unordered = NotEqual | UnorderedFlag,
		GreaterThanOrEqual_Unsigned = GreaterThanOrEqual | UnsignedFlag,
		GreaterThanOrEqual_Unordered = GreaterThanOrEqual | UnorderedFlag,
		GreaterThan_Unsigned = GreaterThan | UnsignedFlag,
		GreaterThan_Unordered = GreaterThan | UnorderedFlag,
		LessThanOrEqual_Unsigned = LessThanOrEqual | UnsignedFlag,
		LessThanOrEqual_Unordered = LessThanOrEqual | UnorderedFlag,
		LessThan_Unsigned = LessThan | UnsignedFlag,
		LessThan_Unordered = LessThan | UnorderedFlag,
	}

	public static class ComparisonExtensions
	{
		public static bool HasCompareOpcode(this Comparison comparison)
		{
			switch (comparison & Comparison.OperationMask)
			{
				case Comparison.Equal:
				case Comparison.GreaterThan:
				case Comparison.LessThan:
					return true;

				default:
					return false;
			}
		}

		public static bool IsUnsignedOrUnordered(this Comparison comparison)
		{
			return (comparison & (Comparison.UnsignedFlag | Comparison.UnorderedFlag)) != 0;
		}
	}
}
