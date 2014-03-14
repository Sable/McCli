using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliKit.Cil
{
	/// <summary>
	/// Implements CLI conversion rules.
	/// </summary>
	public static class Conversions
	{
		public static Conversion? Get(DataType source, DataType target, out bool verifiable)
		{
			verifiable = true;
			if (!source.IsStackTypeExceptValueType() || !target.IsNumeric())
				return null;

			// §III.1.5 Table 8: Conversion Operations
			if (source == target) return Conversion.Identity;

			if (source.IsReferenceOrManagedPointer())
			{
				if (!target.IsNativeOr64BitsInteger()) return null;
				verifiable = false; // Stops GC tracking
				return Conversion.Identity;
			}

			if (source.IsFloat()) return target.IsFloat() ? Conversion.ChangePrecision : Conversion.FloatToInt;
			if (target.IsFloat()) return Conversion.IntToFloat;

			// At this point we're left with
			// Source: int32, int64, native int
			// Target: [u]int**, native [u]int
			int? sourceSize = source.GetSizeInBytes();
			int? targetSize = target.GetSizeInBytes();
			if (sourceSize == targetSize) return Conversion.Identity;

			if ((targetSize ?? 4) < (sourceSize ?? 8)) return Conversion.Truncate;
			return target.IsSignedInteger() ? Conversion.SignExtend : Conversion.ZeroExtend;
		}
	}
}
