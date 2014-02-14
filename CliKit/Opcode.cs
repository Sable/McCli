using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CliKit
{
	/// <summary>
	/// Base class for CIL opcodes.
	/// Instances are immutable and reference comparable.
	/// </summary>
	public class Opcode
	{
		#region Instance
		#region Fields
		protected OpCode opcode;
		#endregion

		#region Constructors
		internal Opcode(OpCode opcode)
		{
			this.opcode = opcode;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the name of this opcode.
		/// </summary>
		public string Name
		{
			get { return opcode.Name; }
		}

		/// <summary>
		/// Gets the code of this opcode.
		/// </summary>
		public short Code
		{
			get { return opcode.Value; }
		}

		/// <summary>
		/// Gets the first byte of this opcode's value.
		/// </summary>
		public byte FirstByte
		{
			get { return unchecked((byte)Code); }
		}

		/// <summary>
		/// Gets the second byte of this opcode's value.
		/// </summary>
		public byte SecondByte
		{
			get
			{
				Contract.Requires(HasTwoBytes);
				return unchecked((byte)(Code >> 8));
			}
		}

		/// <summary>
		/// Gets a value indicating if this opcode's value is encoded on two bytes.
		/// </summary>
		public bool HasTwoBytes
		{
			get { return IsFirstOfTwoBytes(FirstByte); }
		}

		/// <summary>
		/// Gets the number of values popped from the evaluation stack by this opcode,
		/// or <c>null</c> if this opcode pops a variable number of values.
		/// </summary>
		public int? PopCount
		{
			get { return -ReflectionEmitEnums.GetStackDelta(opcode.StackBehaviourPop); }
		}

		/// <summary>
		/// Gets the number of values pushed on the evaluation stack by this opcode,
		/// or <c>null</c> if this opcode pushes a variable number of values.
		/// </summary>
		public int? PushCount
		{
			get { return ReflectionEmitEnums.GetStackDelta(opcode.StackBehaviourPush); }
		}

		/// <summary>
		/// Gets the change in evaluation stack depth incurred by the execution of an instruction
		/// with this opcode, or <c>null</c> if this opcode pushes or pops a variable number of values.
		/// </summary>
		public int? StackDepthDelta
		{
			get
			{
				return ReflectionEmitEnums.GetStackDelta(opcode.StackBehaviourPop)
					+ ReflectionEmitEnums.GetStackDelta(opcode.StackBehaviourPush);
			}
		}

		/// <summary>
		/// Gets the size in bytes of any inline operand this opcode may have.
		/// Returns <c>null</c> if the opcode has a variable-sized inline operand.
		/// </summary>
		public int? OperandSizeInBytes
		{
			get
			{
				switch (opcode.OperandType)
				{
					case OperandType.InlineNone:
						return 0;

					case OperandType.ShortInlineBrTarget:
					case OperandType.ShortInlineI:
					case OperandType.ShortInlineVar:
						return 1;

					case OperandType.InlineVar:
						return 2;

					case OperandType.InlineI:
					case OperandType.ShortInlineR:
					case OperandType.InlineTok:
					case OperandType.InlineSig:
					case OperandType.InlineString:
					case OperandType.InlineType:
					case OperandType.InlineField:
					case OperandType.InlineMethod:
					case OperandType.InlineBrTarget:
						return 4;

					case OperandType.InlineI8:
					case OperandType.InlineR:
						return 8;

					case OperandType.InlineSwitch:
						return null; // Variable-length jump table

					default: throw new NotSupportedException();
				}
			}
		}

		/// <summary>
		/// Gets the size in bytes of an instruction with this opcode,
		/// or <c>null</c> if instructions are variable-sized.
		/// </summary>
		public int? InstructionSizeInBytes
		{
			get { return (HasTwoBytes ? 2 : 1) + OperandSizeInBytes; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Gets the <see cref="System.Reflection.OpCode"/> for this opcode.
		/// </summary>
		/// <param name="opCode">The BCL opcode.</param>
		public void GetReflectionEmitOpCode(out OpCode opCode)
		{
			opCode = this.opcode;
		}

		public override string ToString()
		{
			return Name;
		}
		#endregion
		#endregion

		#region Static
		#region Fields
		public static readonly ArithmeticOpcode Add = new ArithmeticOpcode(OpCodes.Add, ArithmeticOperation.Addition);
		public static readonly ArithmeticOpcode Add_Ovf = new ArithmeticOpcode(OpCodes.Add_Ovf, ArithmeticOperation.Addition_OverflowCheck);
		public static readonly ArithmeticOpcode Add_Ovf_Un = new ArithmeticOpcode(OpCodes.Add_Ovf_Un, ArithmeticOperation.Addition_UnsignedWithOverflowCheck);
		public static readonly ArithmeticOpcode And = new ArithmeticOpcode(OpCodes.And, ArithmeticOperation.BitwiseAnd);
		public static readonly BranchOpcode Beq = new BranchOpcode(OpCodes.Beq, Comparison.Equal);
		public static readonly BranchOpcode Beq_S = new BranchOpcode(OpCodes.Beq_S, Comparison.Equal);
		public static readonly BranchOpcode Bge = new BranchOpcode(OpCodes.Bge, Comparison.GreaterThanOrEqual);
		public static readonly BranchOpcode Bge_S = new BranchOpcode(OpCodes.Bge_S, Comparison.GreaterThanOrEqual);
		public static readonly BranchOpcode Bge_Un = new BranchOpcode(OpCodes.Bge_Un, Comparison.GreaterThanOrEqual_Unsigned);
		public static readonly BranchOpcode Bge_Un_S = new BranchOpcode(OpCodes.Bge_Un_S, Comparison.GreaterThanOrEqual_Unsigned);
		public static readonly BranchOpcode Bgt = new BranchOpcode(OpCodes.Bgt, Comparison.GreaterThan);
		public static readonly BranchOpcode Bgt_S = new BranchOpcode(OpCodes.Bgt_S, Comparison.GreaterThan);
		public static readonly BranchOpcode Bgt_Un = new BranchOpcode(OpCodes.Bgt_Un, Comparison.GreaterThan_Unsigned);
		public static readonly BranchOpcode Bgt_Un_S = new BranchOpcode(OpCodes.Bgt_Un_S, Comparison.GreaterThan_Unsigned);
		public static readonly BranchOpcode Ble = new BranchOpcode(OpCodes.Ble, Comparison.LessThanOrEqual);
		public static readonly BranchOpcode Ble_S = new BranchOpcode(OpCodes.Ble_S, Comparison.LessThanOrEqual);
		public static readonly BranchOpcode Ble_Un = new BranchOpcode(OpCodes.Ble_Un, Comparison.LessThanOrEqual_Unsigned);
		public static readonly BranchOpcode Ble_Un_S = new BranchOpcode(OpCodes.Ble_Un_S, Comparison.LessThanOrEqual_Unsigned);
		public static readonly BranchOpcode Blt = new BranchOpcode(OpCodes.Blt, Comparison.LessThan);
		public static readonly BranchOpcode Blt_S = new BranchOpcode(OpCodes.Blt_S, Comparison.LessThan);
		public static readonly BranchOpcode Blt_Un = new BranchOpcode(OpCodes.Blt_Un, Comparison.LessThan_Unsigned);
		public static readonly BranchOpcode Blt_Un_S = new BranchOpcode(OpCodes.Blt_Un_S, Comparison.LessThan_Unsigned);
		public static readonly BranchOpcode Bne_Un = new BranchOpcode(OpCodes.Bne_Un, Comparison.NotEqual_Unsigned);
		public static readonly BranchOpcode Bne_Un_S = new BranchOpcode(OpCodes.Bne_Un_S, Comparison.NotEqual_Unsigned);
		public static readonly BranchOpcode Br = new BranchOpcode(OpCodes.Br);
		public static readonly BranchOpcode Br_S = new BranchOpcode(OpCodes.Br_S);
		public static readonly BranchOpcode Brfalse = new BranchOpcode(OpCodes.Brfalse);
		public static readonly BranchOpcode Brfalse_S = new BranchOpcode(OpCodes.Brfalse_S);
		public static readonly BranchOpcode Brtrue = new BranchOpcode(OpCodes.Brtrue);
		public static readonly BranchOpcode Brtrue_S = new BranchOpcode(OpCodes.Brtrue_S);
		public static readonly CallOpcode Call = new CallOpcode(OpCodes.Call);
		public static readonly CallOpcode Calli = new CallOpcode(OpCodes.Calli);
		public static readonly CallOpcode Callvirt = new CallOpcode(OpCodes.Callvirt);
		public static readonly Opcode Castclass = new Opcode(OpCodes.Castclass);
		public static readonly ComparisonOpcode Ceq = new ComparisonOpcode(OpCodes.Ceq, Comparison.Equal);
		public static readonly ComparisonOpcode Cgt = new ComparisonOpcode(OpCodes.Cgt, Comparison.GreaterThan);
		public static readonly ComparisonOpcode Cgt_Un = new ComparisonOpcode(OpCodes.Cgt_Un, Comparison.GreaterThan_Unsigned);
		public static readonly ComparisonOpcode Clt = new ComparisonOpcode(OpCodes.Clt, Comparison.LessThan);
		public static readonly ComparisonOpcode Clt_Un = new ComparisonOpcode(OpCodes.Clt_Un, Comparison.LessThan_Unsigned);
		public static readonly ConversionOpcode Conv_I = new ConversionOpcode(OpCodes.Conv_I);
		public static readonly ConversionOpcode Conv_I1 = new ConversionOpcode(OpCodes.Conv_I1);
		public static readonly ConversionOpcode Conv_I2 = new ConversionOpcode(OpCodes.Conv_I2);
		public static readonly ConversionOpcode Conv_I4 = new ConversionOpcode(OpCodes.Conv_I4);
		public static readonly ConversionOpcode Conv_I8 = new ConversionOpcode(OpCodes.Conv_I8);
		public static readonly ConversionOpcode Conv_Ovf_I = new ConversionOpcode(OpCodes.Conv_Ovf_I);
		public static readonly ConversionOpcode Conv_Ovf_I_Un = new ConversionOpcode(OpCodes.Conv_Ovf_I_Un);
		public static readonly ConversionOpcode Conv_Ovf_I1 = new ConversionOpcode(OpCodes.Conv_Ovf_I1);
		public static readonly ConversionOpcode Conv_Ovf_I1_Un = new ConversionOpcode(OpCodes.Conv_Ovf_I1_Un);
		public static readonly ConversionOpcode Conv_Ovf_I2 = new ConversionOpcode(OpCodes.Conv_Ovf_I2);
		public static readonly ConversionOpcode Conv_Ovf_I2_Un = new ConversionOpcode(OpCodes.Conv_Ovf_I2_Un);
		public static readonly ConversionOpcode Conv_Ovf_I4 = new ConversionOpcode(OpCodes.Conv_Ovf_I4);
		public static readonly ConversionOpcode Conv_Ovf_I4_Un = new ConversionOpcode(OpCodes.Conv_Ovf_I4_Un);
		public static readonly ConversionOpcode Conv_Ovf_I8 = new ConversionOpcode(OpCodes.Conv_Ovf_I8);
		public static readonly ConversionOpcode Conv_Ovf_I8_Un = new ConversionOpcode(OpCodes.Conv_Ovf_I8_Un);
		public static readonly ConversionOpcode Conv_Ovf_U = new ConversionOpcode(OpCodes.Conv_Ovf_U);
		public static readonly ConversionOpcode Conv_Ovf_U_Un = new ConversionOpcode(OpCodes.Conv_Ovf_U_Un);
		public static readonly ConversionOpcode Conv_Ovf_U1 = new ConversionOpcode(OpCodes.Conv_Ovf_U1);
		public static readonly ConversionOpcode Conv_Ovf_U1_Un = new ConversionOpcode(OpCodes.Conv_Ovf_U1_Un);
		public static readonly ConversionOpcode Conv_Ovf_U2 = new ConversionOpcode(OpCodes.Conv_Ovf_U2);
		public static readonly ConversionOpcode Conv_Ovf_U2_Un = new ConversionOpcode(OpCodes.Conv_Ovf_U2_Un);
		public static readonly ConversionOpcode Conv_Ovf_U4 = new ConversionOpcode(OpCodes.Conv_Ovf_U4);
		public static readonly ConversionOpcode Conv_Ovf_U4_Un = new ConversionOpcode(OpCodes.Conv_Ovf_U4_Un);
		public static readonly ConversionOpcode Conv_Ovf_U8 = new ConversionOpcode(OpCodes.Conv_Ovf_U8);
		public static readonly ConversionOpcode Conv_Ovf_U8_Un = new ConversionOpcode(OpCodes.Conv_Ovf_U8_Un);
		public static readonly ConversionOpcode Conv_R_Un = new ConversionOpcode(OpCodes.Conv_R_Un);
		public static readonly ConversionOpcode Conv_R4 = new ConversionOpcode(OpCodes.Conv_R4);
		public static readonly ConversionOpcode Conv_R8 = new ConversionOpcode(OpCodes.Conv_R8);
		public static readonly ConversionOpcode Conv_U = new ConversionOpcode(OpCodes.Conv_U);
		public static readonly ConversionOpcode Conv_U1 = new ConversionOpcode(OpCodes.Conv_U1);
		public static readonly ConversionOpcode Conv_U2 = new ConversionOpcode(OpCodes.Conv_U2);
		public static readonly ConversionOpcode Conv_U4 = new ConversionOpcode(OpCodes.Conv_U4);
		public static readonly ConversionOpcode Conv_U8 = new ConversionOpcode(OpCodes.Conv_U8);
		public static readonly ArithmeticOpcode Div = new ArithmeticOpcode(OpCodes.Div, ArithmeticOperation.Division);
		public static readonly ArithmeticOpcode Div_Un = new ArithmeticOpcode(OpCodes.Div_Un, ArithmeticOperation.Division_Unsigned);
		public static readonly Opcode Dup = new Opcode(OpCodes.Dup);
		public static readonly CallOpcode Jmp = new CallOpcode(OpCodes.Jmp);
		public static readonly VariableReferenceOpcode Ldarg = new VariableReferenceOpcode(OpCodes.Ldarg);
		public static readonly VariableReferenceOpcode Ldarg_0 = new VariableReferenceOpcode(OpCodes.Ldarg_0);
		public static readonly VariableReferenceOpcode Ldarg_1 = new VariableReferenceOpcode(OpCodes.Ldarg_1);
		public static readonly VariableReferenceOpcode Ldarg_2 = new VariableReferenceOpcode(OpCodes.Ldarg_2);
		public static readonly VariableReferenceOpcode Ldarg_3 = new VariableReferenceOpcode(OpCodes.Ldarg_3);
		public static readonly VariableReferenceOpcode Ldarg_S = new VariableReferenceOpcode(OpCodes.Ldarg_S);
		public static readonly VariableReferenceOpcode Ldarga = new VariableReferenceOpcode(OpCodes.Ldarga);
		public static readonly VariableReferenceOpcode Ldarga_S = new VariableReferenceOpcode(OpCodes.Ldarga_S);
		public static readonly LoadConstantOpcode Ldc_I4 = new LoadConstantOpcode(OpCodes.Ldc_I4);
		public static readonly LoadConstantOpcode Ldc_I4_0 = new LoadConstantOpcode(OpCodes.Ldc_I4_0);
		public static readonly LoadConstantOpcode Ldc_I4_1 = new LoadConstantOpcode(OpCodes.Ldc_I4_1);
		public static readonly LoadConstantOpcode Ldc_I4_2 = new LoadConstantOpcode(OpCodes.Ldc_I4_2);
		public static readonly LoadConstantOpcode Ldc_I4_3 = new LoadConstantOpcode(OpCodes.Ldc_I4_3);
		public static readonly LoadConstantOpcode Ldc_I4_4 = new LoadConstantOpcode(OpCodes.Ldc_I4_4);
		public static readonly LoadConstantOpcode Ldc_I4_5 = new LoadConstantOpcode(OpCodes.Ldc_I4_5);
		public static readonly LoadConstantOpcode Ldc_I4_6 = new LoadConstantOpcode(OpCodes.Ldc_I4_6);
		public static readonly LoadConstantOpcode Ldc_I4_7 = new LoadConstantOpcode(OpCodes.Ldc_I4_7);
		public static readonly LoadConstantOpcode Ldc_I4_8 = new LoadConstantOpcode(OpCodes.Ldc_I4_8);
		public static readonly LoadConstantOpcode Ldc_I4_M1 = new LoadConstantOpcode(OpCodes.Ldc_I4_M1);
		public static readonly LoadConstantOpcode Ldc_I4_S = new LoadConstantOpcode(OpCodes.Ldc_I4_S);
		public static readonly LoadConstantOpcode Ldc_I8 = new LoadConstantOpcode(OpCodes.Ldc_I8);
		public static readonly LoadConstantOpcode Ldc_R4 = new LoadConstantOpcode(OpCodes.Ldc_R4);
		public static readonly LoadConstantOpcode Ldc_R8 = new LoadConstantOpcode(OpCodes.Ldc_R8);
		public static readonly ElementReferenceOpcode Ldelem = new ElementReferenceOpcode(OpCodes.Ldelem);
		public static readonly ElementReferenceOpcode Ldelem_I = new ElementReferenceOpcode(OpCodes.Ldelem_I);
		public static readonly ElementReferenceOpcode Ldelem_I1 = new ElementReferenceOpcode(OpCodes.Ldelem_I1);
		public static readonly ElementReferenceOpcode Ldelem_I2 = new ElementReferenceOpcode(OpCodes.Ldelem_I2);
		public static readonly ElementReferenceOpcode Ldelem_I4 = new ElementReferenceOpcode(OpCodes.Ldelem_I4);
		public static readonly ElementReferenceOpcode Ldelem_I8 = new ElementReferenceOpcode(OpCodes.Ldelem_I8);
		public static readonly ElementReferenceOpcode Ldelem_R4 = new ElementReferenceOpcode(OpCodes.Ldelem_R4);
		public static readonly ElementReferenceOpcode Ldelem_R8 = new ElementReferenceOpcode(OpCodes.Ldelem_R8);
		public static readonly ElementReferenceOpcode Ldelem_Ref = new ElementReferenceOpcode(OpCodes.Ldelem_Ref);
		public static readonly ElementReferenceOpcode Ldelem_U1 = new ElementReferenceOpcode(OpCodes.Ldelem_U1);
		public static readonly ElementReferenceOpcode Ldelem_U2 = new ElementReferenceOpcode(OpCodes.Ldelem_U2);
		public static readonly ElementReferenceOpcode Ldelem_U4 = new ElementReferenceOpcode(OpCodes.Ldelem_U4);
		public static readonly ElementReferenceOpcode Ldelema = new ElementReferenceOpcode(OpCodes.Ldelema);
		public static readonly FieldReferenceOpcode Ldfld = new FieldReferenceOpcode(OpCodes.Ldfld);
		public static readonly FieldReferenceOpcode Ldflda = new FieldReferenceOpcode(OpCodes.Ldflda);
		public static readonly IndirectReferenceOpcode Ldind_I = new IndirectReferenceOpcode(OpCodes.Ldind_I);
		public static readonly IndirectReferenceOpcode Ldind_I1 = new IndirectReferenceOpcode(OpCodes.Ldind_I1);
		public static readonly IndirectReferenceOpcode Ldind_I2 = new IndirectReferenceOpcode(OpCodes.Ldind_I2);
		public static readonly IndirectReferenceOpcode Ldind_I4 = new IndirectReferenceOpcode(OpCodes.Ldind_I4);
		public static readonly IndirectReferenceOpcode Ldind_I8 = new IndirectReferenceOpcode(OpCodes.Ldind_I8);
		public static readonly IndirectReferenceOpcode Ldind_R4 = new IndirectReferenceOpcode(OpCodes.Ldind_R4);
		public static readonly IndirectReferenceOpcode Ldind_R8 = new IndirectReferenceOpcode(OpCodes.Ldind_R8);
		public static readonly IndirectReferenceOpcode Ldind_Ref = new IndirectReferenceOpcode(OpCodes.Ldind_Ref);
		public static readonly IndirectReferenceOpcode Ldind_U1 = new IndirectReferenceOpcode(OpCodes.Ldind_U1);
		public static readonly IndirectReferenceOpcode Ldind_U2 = new IndirectReferenceOpcode(OpCodes.Ldind_U2);
		public static readonly IndirectReferenceOpcode Ldind_U4 = new IndirectReferenceOpcode(OpCodes.Ldind_U4);
		public static readonly Opcode Ldlen = new Opcode(OpCodes.Ldlen);
		public static readonly VariableReferenceOpcode Ldloc = new VariableReferenceOpcode(OpCodes.Ldloc);
		public static readonly VariableReferenceOpcode Ldloc_0 = new VariableReferenceOpcode(OpCodes.Ldloc_0);
		public static readonly VariableReferenceOpcode Ldloc_1 = new VariableReferenceOpcode(OpCodes.Ldloc_1);
		public static readonly VariableReferenceOpcode Ldloc_2 = new VariableReferenceOpcode(OpCodes.Ldloc_2);
		public static readonly VariableReferenceOpcode Ldloc_3 = new VariableReferenceOpcode(OpCodes.Ldloc_3);
		public static readonly VariableReferenceOpcode Ldloc_S = new VariableReferenceOpcode(OpCodes.Ldloc_S);
		public static readonly VariableReferenceOpcode Ldloca = new VariableReferenceOpcode(OpCodes.Ldloca);
		public static readonly VariableReferenceOpcode Ldloca_S = new VariableReferenceOpcode(OpCodes.Ldloca_S);
		public static readonly FieldReferenceOpcode Ldsfld = new FieldReferenceOpcode(OpCodes.Ldsfld);
		public static readonly FieldReferenceOpcode Ldsflda = new FieldReferenceOpcode(OpCodes.Ldsflda);
		public static readonly LoadConstantOpcode Ldstr = new LoadConstantOpcode(OpCodes.Ldstr);
		public static readonly ArithmeticOpcode Mul = new ArithmeticOpcode(OpCodes.Mul, ArithmeticOperation.Multiplication);
		public static readonly ArithmeticOpcode Mul_Ovf = new ArithmeticOpcode(OpCodes.Mul_Ovf, ArithmeticOperation.Multiplication_OverflowCheck);
		public static readonly ArithmeticOpcode Mul_Ovf_Un = new ArithmeticOpcode(OpCodes.Mul_Ovf_Un, ArithmeticOperation.Multiplication_UnsignedWithOverflowCheck);
		public static readonly ArithmeticOpcode Neg = new ArithmeticOpcode(OpCodes.Neg, ArithmeticOperation.Negate);
		public static readonly CallOpcode Newobj = new CallOpcode(OpCodes.Newobj);
		public static readonly Opcode Nop = new Opcode(OpCodes.Nop);
		public static readonly ArithmeticOpcode Not = new ArithmeticOpcode(OpCodes.Not, ArithmeticOperation.BitwiseNot);
		public static readonly ArithmeticOpcode Or = new ArithmeticOpcode(OpCodes.Or, ArithmeticOperation.BitwiseOr);
		public static readonly Opcode Pop = new Opcode(OpCodes.Pop);
		public static readonly ArithmeticOpcode Rem = new ArithmeticOpcode(OpCodes.Rem, ArithmeticOperation.Remainder);
		public static readonly ArithmeticOpcode Rem_Un = new ArithmeticOpcode(OpCodes.Rem_Un, ArithmeticOperation.Remainder_Unsigned);
		public static readonly Opcode Ret = new Opcode(OpCodes.Ret);
		public static readonly ArithmeticOpcode Shl = new ArithmeticOpcode(OpCodes.Shl, ArithmeticOperation.BitShiftLeft);
		public static readonly ArithmeticOpcode Shr = new ArithmeticOpcode(OpCodes.Shr, ArithmeticOperation.BitShiftRight);
		public static readonly ArithmeticOpcode Shr_Un = new ArithmeticOpcode(OpCodes.Shr_Un, ArithmeticOperation.BitShiftRight_Unsigned);
		public static readonly VariableReferenceOpcode Starg = new VariableReferenceOpcode(OpCodes.Starg);
		public static readonly VariableReferenceOpcode Starg_S = new VariableReferenceOpcode(OpCodes.Starg_S);
		public static readonly ElementReferenceOpcode Stelem = new ElementReferenceOpcode(OpCodes.Stelem);
		public static readonly ElementReferenceOpcode Stelem_I = new ElementReferenceOpcode(OpCodes.Stelem_I);
		public static readonly ElementReferenceOpcode Stelem_I1 = new ElementReferenceOpcode(OpCodes.Stelem_I1);
		public static readonly ElementReferenceOpcode Stelem_I2 = new ElementReferenceOpcode(OpCodes.Stelem_I2);
		public static readonly ElementReferenceOpcode Stelem_I4 = new ElementReferenceOpcode(OpCodes.Stelem_I4);
		public static readonly ElementReferenceOpcode Stelem_I8 = new ElementReferenceOpcode(OpCodes.Stelem_I8);
		public static readonly ElementReferenceOpcode Stelem_R4 = new ElementReferenceOpcode(OpCodes.Stelem_R4);
		public static readonly ElementReferenceOpcode Stelem_R8 = new ElementReferenceOpcode(OpCodes.Stelem_R8);
		public static readonly ElementReferenceOpcode Stelem_Ref = new ElementReferenceOpcode(OpCodes.Stelem_Ref);
		public static readonly FieldReferenceOpcode Stfld = new FieldReferenceOpcode(OpCodes.Stfld);
		public static readonly IndirectReferenceOpcode Stind_I = new IndirectReferenceOpcode(OpCodes.Stind_I);
		public static readonly IndirectReferenceOpcode Stind_I1 = new IndirectReferenceOpcode(OpCodes.Stind_I1);
		public static readonly IndirectReferenceOpcode Stind_I2 = new IndirectReferenceOpcode(OpCodes.Stind_I2);
		public static readonly IndirectReferenceOpcode Stind_I4 = new IndirectReferenceOpcode(OpCodes.Stind_I4);
		public static readonly IndirectReferenceOpcode Stind_I8 = new IndirectReferenceOpcode(OpCodes.Stind_I8);
		public static readonly IndirectReferenceOpcode Stind_R4 = new IndirectReferenceOpcode(OpCodes.Stind_R4);
		public static readonly IndirectReferenceOpcode Stind_R8 = new IndirectReferenceOpcode(OpCodes.Stind_R8);
		public static readonly IndirectReferenceOpcode Stind_Ref = new IndirectReferenceOpcode(OpCodes.Stind_Ref);
		public static readonly VariableReferenceOpcode Stloc = new VariableReferenceOpcode(OpCodes.Stloc);
		public static readonly VariableReferenceOpcode Stloc_S = new VariableReferenceOpcode(OpCodes.Stloc_S);
		public static readonly FieldReferenceOpcode Stsfld = new FieldReferenceOpcode(OpCodes.Stsfld);
		public static readonly ArithmeticOpcode Sub = new ArithmeticOpcode(OpCodes.Sub, ArithmeticOperation.Subtraction);
		public static readonly ArithmeticOpcode Sub_Ovf = new ArithmeticOpcode(OpCodes.Sub_Ovf, ArithmeticOperation.Subtraction_OverflowCheck);
		public static readonly ArithmeticOpcode Sub_Ovf_Un = new ArithmeticOpcode(OpCodes.Sub_Ovf_Un, ArithmeticOperation.Subtraction_UnsignedWithOverflowCheck);
		public static readonly ArithmeticOpcode Xor = new ArithmeticOpcode(OpCodes.Xor, ArithmeticOperation.BitwiseXor);
		#endregion

		#region Methods
		public static bool IsFirstOfTwoBytes(byte @byte)
		{
			return @byte == 0xFE;
		}

		#region Factory Methods
		#endregion
		#endregion
		#endregion
	}
}
