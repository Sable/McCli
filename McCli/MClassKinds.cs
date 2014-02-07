using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// Identifies one or more MatLab classes.
	/// </summary>
	[Flags]
	public enum MClassKinds
	{
		None = 0,

		Double = 1 << 0,
		Single = 1 << 1,

		Int8 = 1 << 2,
		Int16 = 1 << 3,
		Int32 = 1 << 4,
		Int64 = 1 << 5,

		UInt8 = 1 << 6,
		UInt16 = 1 << 7,
		UInt32 = 1 << 8,
		UInt64 = 1 << 9,

		Logical = 1 << 10,
		Char = 1 << 11,

		Cell = 1 << 12,
		Struct = 1 << 13,
		FunctionHandle = 1 << 14,

		FloatMask = Double | Single,
		SignedIntegerMask = Int8 | Int16 | Int32 | Int64,
		UnsignedIntegerMask = UInt8 | UInt16 | UInt32 | UInt64,
		IntegerMask = SignedIntegerMask | UnsignedIntegerMask,
		NumericMask = IntegerMask | FloatMask,
		SupportsComplexMask = NumericMask,
		PrimitiveMask = NumericMask | Logical | Char,
	}
}
