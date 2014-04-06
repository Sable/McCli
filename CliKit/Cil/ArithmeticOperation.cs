using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace CliKit.Cil
{
	[Serializable]
	public enum ArithmeticOperation
	{
		Addition,
		Subtraction,
		Multiplication,
		Division,
		Remainder,
		Negate,

		BitwiseAnd,
		BitwiseOr,
		BitwiseXor,
		BitShiftLeft,
		BitShiftRight,
		BitwiseNot,

		OperationMask = 0xF,

		UnsignedFlag = 0x10,
		OverflowCheckFlag = 0x20,
		ModifiersMask = UnsignedFlag | OverflowCheckFlag,

		Addition_OverflowCheck = Addition | OverflowCheckFlag,
		Addition_UnsignedWithOverflowCheck = Addition | UnsignedFlag | OverflowCheckFlag,
		Subtraction_OverflowCheck = Subtraction | OverflowCheckFlag,
		Subtraction_UnsignedWithOverflowCheck = Subtraction | UnsignedFlag | OverflowCheckFlag,
		Multiplication_OverflowCheck = Multiplication | OverflowCheckFlag,
		Multiplication_UnsignedWithOverflowCheck = Multiplication | UnsignedFlag | OverflowCheckFlag,
		Division_Unsigned = Division | UnsignedFlag,
		Remainder_Unsigned = Remainder | UnsignedFlag,
		BitShiftRight_Unsigned = BitShiftRight | UnsignedFlag,
	}

	public static class ArithmeticOperationEnum
	{
		[Pure]
		public static bool IsUnary(this ArithmeticOperation operation)
		{
			operation &= ArithmeticOperation.OperationMask;
			return operation == ArithmeticOperation.Negate || operation == ArithmeticOperation.BitwiseNot;
		}

		[Pure]
		public static bool IsBinary(this ArithmeticOperation operation)
		{
			return !IsUnary(operation);
		}

		[Pure]
		public static int GetArity(this ArithmeticOperation operation)
		{
			return IsUnary(operation) ? 1 : 2;
		}

		[Pure]
		public static bool IsUnsigned(this ArithmeticOperation operation)
		{
			return (operation & ArithmeticOperation.UnsignedFlag) != 0;
		}

		[Pure]
		public static bool IsOverflowChecked(this ArithmeticOperation operation)
		{
			return (operation & ArithmeticOperation.OverflowCheckFlag) != 0;
		}

		[Pure]
		public static bool IsValidOnFloats(this ArithmeticOperation operation)
		{
			// Unsigned/OverflowCheck modifiers are not supported
			switch (operation)
			{
				case ArithmeticOperation.Addition:
				case ArithmeticOperation.Subtraction:
				case ArithmeticOperation.Multiplication:
				case ArithmeticOperation.Division:
				case ArithmeticOperation.Remainder:
				case ArithmeticOperation.Negate:
					return true;

				default:
					return false;
			}
		}
		
		public static DataType? GetResult(this ArithmeticOperation operation, DataType operand)
		{
			Contract.Requires(operation.IsUnary());
			Contract.Requires(operand.IsStackType());

			// ECMA 335 III.1.5 Table 3: Unary Numeric Operations (neg - plus 'not')
			switch (operation)
			{
				case ArithmeticOperation.Negate: return operand.IsNumericStackType() ? operand : (DataType?)null;
				case ArithmeticOperation.BitwiseNot: return operand.IsIntegerStackType() ? operand : (DataType?)null;
				default: throw new ArgumentException("operation");
			}
		}

		public static DataType? GetResult(this ArithmeticOperation operation, DataType lhs, DataType rhs, out bool verifiable)
		{
			Contract.Requires(operation.IsBinary());
			Contract.Requires(lhs.IsStackType());
			Contract.Requires(rhs.IsStackType());

			verifiable = true;

			switch (operation)
			{
				// ECMA 335 III.1.5 Table 2: Binary Numeric Operations (add, div, mul, rem, sub)
				case ArithmeticOperation.Addition:
				case ArithmeticOperation.Division:
				case ArithmeticOperation.Multiplication:
				case ArithmeticOperation.Remainder:
				case ArithmeticOperation.Subtraction:

				// ECMA 335 III.1.5 Table 7: Overflow Arithmetic Operations (add.ovf, add.ovf.un, mul.ovf, mul.ovf.un, sub.ovf, sub.ovf.un)
				case ArithmeticOperation.Addition_OverflowCheck:
				case ArithmeticOperation.Addition_UnsignedWithOverflowCheck:
				case ArithmeticOperation.Multiplication_OverflowCheck:
				case ArithmeticOperation.Multiplication_UnsignedWithOverflowCheck:
				case ArithmeticOperation.Subtraction_OverflowCheck:
				case ArithmeticOperation.Subtraction_UnsignedWithOverflowCheck:

					if (lhs == rhs)
					{
						if (lhs.IsIntegerStackType()) return lhs;
						if (lhs.IsFloat() && !operation.IsOverflowChecked()) return lhs;
						if (lhs.IsManagedPointer() && (operation == ArithmeticOperation.Subtraction
							|| operation == ArithmeticOperation.Subtraction_UnsignedWithOverflowCheck))
						{
							verifiable = false;
							return DataType.NativeInt;
						}
					}
					else
					{
						if ((lhs == DataType.Int32 && rhs == DataType.NativeInt)
							|| (lhs == DataType.NativeInt && rhs == DataType.Int32))
						{
							return DataType.NativeInt;
						}

						if (lhs.IsManagedPointer() && (rhs == DataType.Int32 || rhs == DataType.NativeInt)
							&& (operation == ArithmeticOperation.Addition || operation == ArithmeticOperation.Subtraction
								|| operation == ArithmeticOperation.Addition_UnsignedWithOverflowCheck
								|| operation == ArithmeticOperation.Subtraction_UnsignedWithOverflowCheck))
						{
							verifiable = false;
							return lhs;
						}

						if ((lhs == DataType.Int32 || lhs == DataType.NativeInt) && rhs.IsManagedPointer()
							&& (operation == ArithmeticOperation.Addition || operation == ArithmeticOperation.Addition_UnsignedWithOverflowCheck))
						{
							verifiable = false;
							return rhs;
						}
					}
					break;
					
				// ECMA 335 III.1.5 Table 5: Integer Operations (and, div.un, or, rem.un, xor - 'not' excluded)
				case ArithmeticOperation.BitwiseAnd:
				case ArithmeticOperation.Division_Unsigned:
				case ArithmeticOperation.BitwiseOr:
				case ArithmeticOperation.Remainder_Unsigned:
				case ArithmeticOperation.BitwiseXor:

					if (lhs == rhs)
					{
						if (lhs.IsIntegerStackType()) return lhs;
					}
					else
					{
						if ((lhs == DataType.Int32 && rhs == DataType.NativeInt)
							|| (lhs == DataType.NativeInt && rhs == DataType.Int32))
						{
							return DataType.NativeInt;
						}
					}
					break;
					
				// ECMA 335 III.1.5 Table 6: Shift Operations (shl, shr, shr.un)
				case ArithmeticOperation.BitShiftLeft:
				case ArithmeticOperation.BitShiftRight:
				case ArithmeticOperation.BitShiftRight_Unsigned:

					if (lhs.IsIntegerStackType() && (rhs == DataType.Int32 || rhs == DataType.NativeInt))
						return lhs;
					break;

				default:
					throw new ArgumentException("Invalid arithmetic operation enumerant.", "operation");
			}

			verifiable = false;
			return null;
		}
		
		public static DataType? GetResult(this ArithmeticOperation operation, DataType lhs, DataType rhs)
		{
			bool verifiable;
			return GetResult(operation, lhs, rhs, out verifiable);
		}
	}
}
