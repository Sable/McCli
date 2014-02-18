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
		private const int sizedIntegerMask = signedSizedIntegerMask | unsignedSizedIntegerMask;
		private const int nativeIntegerMask = (1 << (int)DataType.NativeInt) | (1 << (int)DataType.NativeUInt);
		private const int integerMask = signedIntegerMask | unsignedIntegerMask;
		private const int shortIntegerMask = (1 << (int)DataType.Int8) | (1 << (int)DataType.UInt8) | (1 << (int)DataType.Int16) | (1 << (int)DataType.UInt16);
		private const int integer32Mask = (1 << (int)DataType.Int32) | (1 << (int)DataType.UInt32);
		private const int integer64Mask = (1 << (int)DataType.Int64) | (1 << (int)DataType.UInt64);
		private const int becomesInt32OnStackMask = shortIntegerMask | integer32Mask;
		private const int nativeOr64BitIntegerMask = nativeIntegerMask | integer64Mask;
		private const int sizedFloatMask = (1 << (int)DataType.Float32) | (1 << (int)DataType.Float64);
		private const int floatMask = sizedFloatMask | (1 << (int)DataType.NativeFloat);
		private const int numeric32Mask = integer32Mask | (1 << (int)DataType.Float32);
		private const int numeric64Mask = integer64Mask | (1 << (int)DataType.Float64);
		private const int nativeNumericMask = nativeIntegerMask | (1 << (int)DataType.NativeFloat);
		private const int numericMask = integerMask | floatMask;
		private const int managedPointerMask = (1 << (int)DataType.NormalMutabilityManagedPointer) | (1 << (int)DataType.ControlledMutabilityManagedPointer);
		private const int referenceOrManagedPointerMask = (1 << (int)DataType.ObjectReference) | managedPointerMask;
		private const int numericStackTypeMask = (1 << (int)DataType.Int32) | (1 << (int)DataType.Int64)
			| (1 << (int)DataType.NativeInt) | (1 << (int)DataType.NativeFloat);
		private const int stackTypeExceptValueTypeMask = numericStackTypeMask | referenceOrManagedPointerMask;
		private const int stackTypeMask = stackTypeExceptValueTypeMask | (1 << (int)DataType.ValueType);

		private static readonly Dictionary<string, DataType> nameInOpcode
			= new Dictionary<string, DataType>(StringComparer.OrdinalIgnoreCase)
			{
				{ "i", DataType.NativeInt },
				{ "i1", DataType.Int8 },
				{ "i2", DataType.Int16 },
				{ "i4", DataType.Int32 },
				{ "i8", DataType.Int64 },
				{ "r", DataType.NativeFloat },
				{ "r4", DataType.Float32 },
				{ "r8", DataType.Float64 },
				{ "ref", DataType.NormalMutabilityManagedPointer },
				{ "u", DataType.NativeUInt },
				{ "u1", DataType.UInt8 },
				{ "u2", DataType.UInt16 },
				{ "u4", DataType.UInt32 },
				{ "u8", DataType.UInt64 },
			};

		private static readonly Dictionary<Type, DataType> ctsDataTypes
			= new Dictionary<Type, DataType>()
		{
			{ typeof(sbyte), DataType.Int8 },
			{ typeof(short), DataType.Int16 },
			{ typeof(int), DataType.Int32 },
			{ typeof(long), DataType.Int64 },
			{ typeof(IntPtr), DataType.NativeInt },
			{ typeof(byte), DataType.UInt8 },
			{ typeof(ushort), DataType.UInt16 },
			{ typeof(uint), DataType.UInt32 },
			{ typeof(ulong), DataType.UInt64 },
			{ typeof(UIntPtr), DataType.NativeUInt },
			{ typeof(float), DataType.Float32 },
			{ typeof(double), DataType.Float64 },
			{ typeof(char), DataType.UInt16 },
			{ typeof(bool), DataType.UInt8 },
		};

		private static bool MatchesMask(DataType type, int mask)
		{
			return ((1 << (int)type) & mask) != 0;
		}

		[Pure]
		public static DataType? TryParseNameInOpcode(string name)
		{
			Contract.Requires(name != null);
			DataType dataType;
			return nameInOpcode.TryGetValue(name, out dataType) ? dataType : (DataType?)null;
		}

		public static DataType FromCtsType(Type type)
		{
			Contract.Requires(type != null && !type.IsGenericTypeDefinition && !type.IsGenericParameter);

			DataType dataType;
			if (ctsDataTypes.TryGetValue(type, out dataType)) return dataType;

			if (type.IsValueType) return DataType.ValueType;
			if (type.IsByRef) return DataType.NormalMutabilityManagedPointer;
			return DataType.ObjectReference;
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
		public static bool IsShortInteger(this DataType type) { return MatchesMask(type, shortIntegerMask); }
		[Pure]
		public static bool IsInteger32(this DataType type) { return MatchesMask(type, integer32Mask); }
		[Pure]
		public static bool IsInteger64(this DataType type) { return MatchesMask(type, integer64Mask); }
		[Pure]
		public static bool IsNativeInteger(this DataType type) { return MatchesMask(type, nativeIntegerMask); }
		[Pure]
		public static bool IsNativeOr64BitsInteger(this DataType type) { return MatchesMask(type, nativeOr64BitIntegerMask); }
		[Pure]
		public static bool IsFloat(this DataType type) { return MatchesMask(type, floatMask); }
		[Pure]
		public static bool IsNumeric(this DataType type) { return MatchesMask(type, numericMask); }
		[Pure]
		public static bool IsNumeric32(this DataType type) { return MatchesMask(type, numeric32Mask); }
		[Pure]
		public static bool IsNumeric64(this DataType type) { return MatchesMask(type, numeric64Mask); }
		[Pure]
		public static bool IsNativeNumeric(this DataType type) { return MatchesMask(type, nativeNumericMask); }
		[Pure]
		public static bool IsManagedPointer(this DataType type) { return MatchesMask(type, managedPointerMask); }
		[Pure]
		public static bool IsReferenceOrManagedPointer(this DataType type) { return MatchesMask(type, referenceOrManagedPointerMask); }
		[Pure]
		public static bool IsNumericStackType(this DataType type) { return MatchesMask(type, numericStackTypeMask); }
		[Pure]
		public static bool IsStackType(this DataType type) { return MatchesMask(type, stackTypeMask); }
		[Pure]
		public static bool IsStackTypeExceptValueType(this DataType type) { return MatchesMask(type, stackTypeExceptValueTypeMask); }

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
