using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliKit
{
	/// <summary>
	/// Identifies one or more of the "Data Types Directly Supported by the CLI" (ISO 23271 - §I.12.1).
	/// </summary>
	[Flags]
	public enum DataTypes
	{
		None = 0,

		Int8 = 1 << 0,
		UInt8 = 1 << 1,
		Int16 = 1 << 2,
		UInt16 = 1 << 3,
		Int32 = 1 << 4,
		UInt32 = 1 << 5,
		Int64 = 1 << 6,
		UInt64 = 1 << 7,

		Float32 = 1 << 8,
		Float64 = 1 << 9,

		NativeInt = 1 << 10,
		NativeUInt = 1 << 11,
		NativeFloat = 1 << 12,

		ObjectReference = 1 << 13,
		NormalMutabilityManagedPointer = 1 << 14,
		ControlledMutabilityManagedPointer = 1 << 15,
		ValueType = 1 << 16,

		// Other names for existing types
		UnmanagedPointer = NativeUInt,
		Char = UInt16,
		Boolean = UInt8,

		// Masks
		SignedSizedIntMask = Int8 | Int16 | Int32 | Int64,
		SignedIntMask = SignedSizedIntMask | NativeInt,
		UnsignedSizedIntMask = UInt8 | UInt16 | UInt32 | UInt64,
		UnsignedIntMask = UnsignedSizedIntMask | NativeUInt,
		SizedIntMask = SignedSizedIntMask | UnsignedIntMask,
		ShortIntMask = Int8 | UInt8 | Int16 | UInt16,
		NativeIntMask = NativeInt | NativeUInt,
		IntMask = SignedIntMask | UnsignedIntMask,
		SizedFloatMask = Float32 | Float64,
		FloatMask = SizedFloatMask | NativeFloat,
		SizedMask = SizedIntMask | SizedFloatMask,
		NativeNumericMask = NativeIntMask | NativeFloat,
		NumericMask = IntMask | FloatMask,
		ManagedPointerMask = NormalMutabilityManagedPointer | ControlledMutabilityManagedPointer,
		NativeSizedMask = NativeNumericMask | ObjectReference | ManagedPointerMask,
		StackTypeMask = Int32 | Int64 | NativeInt | NativeFloat | ObjectReference | ManagedPointerMask | ValueType
	}

	public static class DataTypesEnum
	{
		[Pure]
		public static DataTypes TryParseNameInOpcode(string name)
		{
			Contract.Requires(name != null);

			throw new NotImplementedException();
		}

		[Pure]
		public static DataTypes ToStackType(this DataTypes value)
		{
			Contract.Requires(IsSingleType(value));

			if (value.IsOneOf(DataTypes.StackTypeMask)) return value;
			if (value.IsOneOf(DataTypes.ShortIntMask | DataTypes.UInt32)) return DataTypes.Int32;
			if (value.IsOneOf(DataTypes.SizedFloatMask)) return DataTypes.NativeFloat;
			if (value == DataTypes.UInt64) return DataTypes.Int64;
			if (value == DataTypes.NativeUInt) return DataTypes.NativeInt;
			throw new ArgumentException("value");
		}

		[Pure]
		public static bool IsSingleType(this DataTypes value)
		{
			// Check if it's a power of two.
			return value != 0 && ((int)value & ((int)value - 1)) == 0;
		}

		[Pure]
		public static bool IsOneOf(this DataTypes value, DataTypes mask)
		{
			return (value & mask) != 0;
		}
	}
}
