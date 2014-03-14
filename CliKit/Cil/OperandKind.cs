using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliKit.Cil
{
	/// <summary>
	/// Identifies a kind of inline operand that accompanies an opcode in a CIL instruction.
	/// </summary>
	public enum OperandKind
	{
		BranchTarget32 = 0,
		FieldToken = 1,
		Int32 = 2,
		Int64 = 3,
		MethodToken = 4,
		None = 5,
		Float64 = 7,
		SignatureToken = 9,
		StringToken = 10,
		JumpTable = 11,
		Token = 12,
		TypeToken = 13,
		VariableIndex16 = 14,
		BranchTarget8 = 15,
		Int8 = 16,
		Float32 = 17,
		VariableIndex8 = 18
	}

	public static class OperandKindEnum
	{
		#region Fields
		private const int enumerantsPerField = 9;
		private const int bitsPerEnumerant = 6;
		private const int lowEnumerantMask = (1 << bitsPerEnumerant) - 1;
		
		private static ulong dataTypes1, dataTypes2;
		private static ulong sizes1, sizes2;
		#endregion

		#region Constructors
		static OperandKindEnum()
		{
			InitializeEnumerant(OperandKind.BranchTarget32, DataType.Int32);
			InitializeEnumerant(OperandKind.FieldToken, DataType.UInt16);
			InitializeEnumerant(OperandKind.Int32, DataType.Int32);
			InitializeEnumerant(OperandKind.Int64, DataType.Int64);
			InitializeEnumerant(OperandKind.MethodToken, DataType.UInt32);
			// OperandKind.None
			InitializeEnumerant(OperandKind.Float64, DataType.Float64);
			InitializeEnumerant(OperandKind.SignatureToken, DataType.UInt32);
			InitializeEnumerant(OperandKind.StringToken, DataType.UInt32);
			// OperandKind.JumpTable
			InitializeEnumerant(OperandKind.Token, DataType.UInt32);
			InitializeEnumerant(OperandKind.TypeToken, DataType.UInt32);
			InitializeEnumerant(OperandKind.VariableIndex16, DataType.UInt16);
			InitializeEnumerant(OperandKind.BranchTarget8, DataType.Int8);
			InitializeEnumerant(OperandKind.Int8, DataType.Int8);
			InitializeEnumerant(OperandKind.Float32, DataType.Float32);
			InitializeEnumerant(OperandKind.VariableIndex8, DataType.UInt8);
		}

		private static void InitializeEnumerant(OperandKind kind, DataType type)
		{
			if ((int)kind < enumerantsPerField)
			{
				int shift = (int)kind * bitsPerEnumerant;
				dataTypes1 = (ulong)type << shift;
				sizes1 = (ulong)type.GetSizeInBytes() << shift;
			}
			else
			{
				int shift = ((int)kind - enumerantsPerField) * bitsPerEnumerant;
				dataTypes2 = (ulong)type << shift;
				sizes2 = (ulong)type.GetSizeInBytes() << shift;
			}
		}
		#endregion

		#region Methods
		public static bool IsVariableIndex(this OperandKind kind)
		{
			const int mask = (1 << (int)OperandKind.VariableIndex8) | (1 << (int)OperandKind.VariableIndex16);
			return ((1 << (int)kind) & mask) != 0;
		}
		
		public static bool IsBranchTarget(this OperandKind kind)
		{
			const int mask = (1 << (int)OperandKind.BranchTarget8) | (1 << (int)OperandKind.BranchTarget32);
			return ((1 << (int)kind) & mask) != 0;
		}

		public static bool IsMetadataToken(this OperandKind kind)
		{
			const int mask = (1 << (int)OperandKind.FieldToken)
				| (1 << (int)OperandKind.MethodToken) | (1 << (int)OperandKind.SignatureToken)
				| (1 << (int)OperandKind.StringToken) | (1 << (int)OperandKind.Token)
				| (1 << (int)OperandKind.TypeToken);
			return ((1 << (int)kind) & mask) != 0;
		}

		public static bool IsConstant(this OperandKind kind)
		{
			const int mask = (1 << (int)OperandKind.Int8)
				| (1 << (int)OperandKind.Int32) | (1 << (int)OperandKind.Int64)
				| (1 << (int)OperandKind.Float32) | (1 << (int)OperandKind.Float64);
			return ((1 << (int)kind) & mask) != 0;
		}

		/// <summary>
		/// Gets the size in bytes of an opcode operand of a given kind when encoded in an instruction stream.
		/// </summary>
		/// <param name="kind">The operand kind.</param>
		/// <returns>
		/// The size of the operand in bytes, 
		/// or <c>null</c> if it is variable-sized (<see cref="OperandKind.JumpTable"/>).
		/// </returns>
		public static int? GetSizeInBytes(this OperandKind kind)
		{
			if ((int)kind < enumerantsPerField)
			{
				int shift = (int)kind * bitsPerEnumerant;
				return (int)(sizes1 >> shift) & lowEnumerantMask;
			}
			else
			{
				int shift = ((int)kind - enumerantsPerField) * bitsPerEnumerant;
				return kind == OperandKind.JumpTable ? (int?)null : ((int)(sizes2 >> shift) & lowEnumerantMask);
			}
		}

		public static DataType? GetDataType(this OperandKind kind)
		{
			if ((int)kind < enumerantsPerField)
			{
				int shift = (int)kind * bitsPerEnumerant;
				return kind == OperandKind.None
					? (DataType?)null
					: (DataType)((int)(dataTypes1 >> shift) & lowEnumerantMask);
			}
			else
			{
				int shift = ((int)kind - enumerantsPerField) * bitsPerEnumerant;
				return kind == OperandKind.JumpTable
					? (DataType?)null
					: (DataType)((int)(dataTypes2 >> shift) & lowEnumerantMask);
			}
		}
		#endregion
	}
}
