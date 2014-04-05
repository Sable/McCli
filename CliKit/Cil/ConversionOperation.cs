using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CliKit.Cil
{
	/// <summary>
	/// Identifies an operation performed to convert a value between CLI data types.
	/// </summary>
	[Serializable]
	public enum ConversionOperation
	{
		/// <summary>
		/// Indicates that no conversion needs to take place.
		/// </summary>
		Identity,

		/// <summary>
		/// Indicates that some of an integer's most significant bits are dropped.
		/// </summary>
		Truncate,

		/// <summary>
		/// Indicates that the integer is widened by adding zeroes.
		/// </summary>
		ZeroExtend,

		/// <summary>
		/// Indicates that the integer is widened by duplicating its sign bit.
		/// </summary>
		SignExtend,

		/// <summary>
		/// Indicates that a floating-point value is converted to an integer value.
		/// </summary>
		FloatToInt,

		/// <summary>
		/// Indicates that an integer value is converted to a floating-point value.
		/// </summary>
		IntToFloat,

		/// <summary>
		/// Indicates that a floating-point value is reencoded with a different precision.
		/// </summary>
		ChangePrecision
	}

	public static class ConversionOperationEnum
	{
		public static ConversionOperation? Get(DataType source, DataType target, out bool verifiable)
		{
			verifiable = true;
			if (!source.IsStackTypeExceptValueType() || !target.IsNumeric())
				return null;

			// §III.1.5 Table 8: Conversion Operations
			if (source == target) return ConversionOperation.Identity;

			if (source.IsReferenceOrManagedPointer())
			{
				if (!target.IsNativeOr64BitsInteger()) return null;
				verifiable = false; // Stops GC tracking
				return ConversionOperation.Identity;
			}

			if (source.IsFloat()) return target.IsFloat() ? ConversionOperation.ChangePrecision : ConversionOperation.FloatToInt;
			if (target.IsFloat()) return ConversionOperation.IntToFloat;

			// At this point we're left with
			// Source: int32, int64, native int
			// Target: [u]int**, native [u]int
			int? sourceSize = source.GetSizeInBytes();
			int? targetSize = target.GetSizeInBytes();
			if (sourceSize == targetSize) return ConversionOperation.Identity;

			if ((targetSize ?? 4) < (sourceSize ?? 8)) return ConversionOperation.Truncate;
			return target.IsSignedInteger() ? ConversionOperation.SignExtend : ConversionOperation.ZeroExtend;
		}
	}
}
