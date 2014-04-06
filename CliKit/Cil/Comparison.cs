using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CliKit.Cil
{
	/// <summary>
	/// Identifies one of the comparison operations defined by the CLI.
	/// </summary>
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

	public static class ComparisonEnum
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

		public static bool IsApplicableTo(this Comparison comparison, DataType first, DataType second, out bool verifiable)
		{
			if (!first.IsStackTypeExceptValueType() || !second.IsStackTypeExceptValueType())
			{
				verifiable = false;
				return false;
			}

			if (first == second)
			{
				if (first != DataType.ObjectReference
					|| comparison == Comparison.Equal
					|| comparison == Comparison.NotEqual_Unordered)
				{
					verifiable = true;
					return true;
				}
			}
			else
			{
				if ((first == DataType.Int32 && second == DataType.NativeInt)
					|| (first == DataType.NativeInt && second == DataType.Int32))
				{
					verifiable = true;
					return true;
				}

				if ((first.IsManagedPointer() && second == DataType.NativeInt)
					|| (first == DataType.NativeInt && second.IsManagedPointer()))
				{
					verifiable = false;
					return true;
				}
			}

			verifiable = false;
			return false;
		}
	}
}
