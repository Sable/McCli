using System;
using System.Collections.Generic;
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

	public static class ArithmeticOperationExtensions
	{
		public static bool IsUnary(this ArithmeticOperation operation)
		{
			operation &= ArithmeticOperation.OperationMask;
			return operation == ArithmeticOperation.Negate || operation == ArithmeticOperation.BitwiseNot;
		}

		public static bool IsBinary(this ArithmeticOperation operation)
		{
			return !IsUnary(operation);
		}

		public static int GetArity(this ArithmeticOperation operation)
		{
			return IsUnary(operation) ? 1 : 2;
		}

		public static bool IsUnsigned(this ArithmeticOperation operation)
		{
			return (operation & ArithmeticOperation.UnsignedFlag) != 0;
		}

		public static bool IsOverflowChecked(this ArithmeticOperation operation)
		{
			return (operation & ArithmeticOperation.OverflowCheckFlag) != 0;
		}

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
	}
}
