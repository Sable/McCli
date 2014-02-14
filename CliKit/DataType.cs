using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliKit
{
	/// <summary>
	/// Identifies one of the "Data Types Directly Supported by the CLI" (ISO 23271 - §I.12.1).
	/// </summary>
	[Flags]
	public enum DataType
	{
		// Check DataTypeEnum methods below if enumerant values change.

		Int8,
		UInt8,
		Int16,
		UInt16,
		Int32,
		UInt32,
		Int64,
		UInt64,

		Float32,
		Float64,

		NativeInt,
		NativeUInt,
		NativeFloat,

		ObjectReference,
		NormalMutabilityManagedPointer,
		ControlledMutabilityManagedPointer,
		ValueType,

		/// <summary>
		/// Alternate name for <see cref="NativeUInt"/>.
		/// </summary>
		UnmanagedPointer = NativeUInt,

		/// <summary>
		/// Alternate name for <see cref="UInt16"/>.
		/// </summary>
		Char = UInt16,

		/// <summary>
		/// Alternate name for <see cref="UInt8"/>.
		/// </summary>
		Boolean = UInt8,
	}

	/// <summary>
	/// Provides utility methods accompanying the <see cref="DataType"/> enumeration.
	/// </summary>
	public static class DataTypeEnum
	{
		private const int signedSizedIntegerMask = (1 << (int)DataType.Int8) | (1 << (int)DataType.Int16) | (1 << (int)DataType.Int32) | (1 << (int)DataType.Int64);
		private const int unsignedSizedIntegerMask = (1 << (int)DataType.UInt8) | (1 << (int)DataType.UInt16) | (1 << (int)DataType.UInt32) | (1 << (int)DataType.UInt64);
		private const int signedIntegerMask = signedSizedIntegerMask | (1 << (int)DataType.NativeInt);
		private const int unsignedIntegerMask = unsignedSizedIntegerMask | (1 << (int)DataType.NativeUInt);
		private const int integerMask = signedIntegerMask | unsignedIntegerMask;
		private const int shortIntegerMask = (1 << (int)DataType.Int8) | (1 << (int)DataType.UInt8) | (1 << (int)DataType.Int16) | (1 << (int)DataType.UInt16);
		private const int becomesInt32OnStackMask = shortIntegerMask | (1 << (int)DataType.Int32) | (1 << (int)DataType.UInt32);
		private const int floatMask = (1 << (int)DataType.Float32) | (1 << (int)DataType.Float64) | (1 << (int)DataType.NativeFloat);
		private const int nativeNumericMask = (1 << (int)DataType.NativeInt) | (1 << (int)DataType.NativeUInt) | (1 << (int)DataType.NativeFloat);
		private const int numericMask = integerMask | floatMask;
		private const int managedPointerMask = (1 << (int)DataType.NormalMutabilityManagedPointer) | (1 << (int)DataType.ControlledMutabilityManagedPointer);
		private const int stackTypeMask = (1 << (int)DataType.Int32) | (1 << (int)DataType.Int64)
			| (1 << (int)DataType.NativeInt) | (1 << (int)DataType.NativeFloat)
			| (1 << (int)DataType.ObjectReference) | managedPointerMask | (1 << (int)DataType.ValueType);

		private static bool MatchesMask(DataType type, int mask)
		{
			return ((1 << (int)type) & mask) != 0;
		}

		[Pure]
		public static DataType TryParseNameInOpcode(string name)
		{
			Contract.Requires(name != null);

			throw new NotImplementedException();
		}

		[Pure]
		public static int? GetSizeInBytes(this DataType type)
		{
			if (type <= DataType.UInt64) return 1 << ((int)type >> 1);
			if (type == DataType.Float32) return 4;
			if (type == DataType.Float64) return 8;
			return null;
		}

		[Pure]
		public static bool IsInteger(this DataType type) { return MatchesMask(type, integerMask); }
		[Pure]
		public static bool IsSignedInteger(this DataType type) { return MatchesMask(type, signedIntegerMask); }
		[Pure]
		public static bool IsUnsignedInteger(this DataType type) { return MatchesMask(type, unsignedIntegerMask); }
		[Pure]
		public static bool IsFloat(this DataType type) { return MatchesMask(type, floatMask); }
		[Pure]
		public static bool IsNumeric(this DataType type) { return MatchesMask(type, numericMask); }
		[Pure]
		public static bool IsManagedPointer(this DataType type) { return MatchesMask(type, managedPointerMask); }

		[Pure]
		public static DataType ToStackType(this DataType type)
		{
			int flag = 1 << (int)type;
			if ((flag & stackTypeMask) != 0) return type;
			if ((flag & becomesInt32OnStackMask) != 0) return DataType.Int32;
			if ((flag & floatMask) != 0) return DataType.NativeFloat;
			if (type == DataType.UInt64) return DataType.Int64;
			if (type == DataType.NativeUInt) return DataType.NativeInt;
			throw new ArgumentException("value");
		}
	}
}
