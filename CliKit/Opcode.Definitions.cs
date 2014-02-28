using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using System.Diagnostics.Contracts;

namespace CliKit
{
	partial class Opcode
	{
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
		public static readonly CastOpcode Box = new CastOpcode(OpCodes.Box);
		public static readonly BranchOpcode Br = new BranchOpcode(OpCodes.Br);
		public static readonly BranchOpcode Br_S = new BranchOpcode(OpCodes.Br_S);
		public static readonly Opcode Break = new Opcode(OpCodes.Break);
		public static readonly BranchOpcode Brfalse = new BranchOpcode(OpCodes.Brfalse);
		public static readonly BranchOpcode Brfalse_S = new BranchOpcode(OpCodes.Brfalse_S);
		public static readonly BranchOpcode Brtrue = new BranchOpcode(OpCodes.Brtrue);
		public static readonly BranchOpcode Brtrue_S = new BranchOpcode(OpCodes.Brtrue_S);
		public static readonly CallOpcode Call = new CallOpcode(OpCodes.Call);
		public static readonly CallOpcode Calli = new CallOpcode(OpCodes.Calli);
		public static readonly CallOpcode Callvirt = new CallOpcode(OpCodes.Callvirt);
		public static readonly CastOpcode Castclass = new CastOpcode(OpCodes.Castclass);
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
		public static readonly CastOpcode Isinst = new CastOpcode(OpCodes.Isinst);
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
		public static readonly LoadConstantOpcode Ldnull = new LoadConstantOpcode(OpCodes.Ldnull);
		public static readonly IndirectReferenceOpcode Ldobj = new IndirectReferenceOpcode(OpCodes.Ldobj);
		public static readonly FieldReferenceOpcode Ldsfld = new FieldReferenceOpcode(OpCodes.Ldsfld);
		public static readonly FieldReferenceOpcode Ldsflda = new FieldReferenceOpcode(OpCodes.Ldsflda);
		public static readonly LoadConstantOpcode Ldstr = new LoadConstantOpcode(OpCodes.Ldstr);
		public static readonly Opcode Ldtoken = new Opcode(OpCodes.Ldtoken);
		public static readonly BranchOpcode Leave = new BranchOpcode(OpCodes.Leave);
		public static readonly BranchOpcode Leave_S = new BranchOpcode(OpCodes.Leave_S);
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
		public static readonly IndirectReferenceOpcode Stobj = new IndirectReferenceOpcode(OpCodes.Stobj);
		public static readonly FieldReferenceOpcode Stsfld = new FieldReferenceOpcode(OpCodes.Stsfld);
		public static readonly ArithmeticOpcode Sub = new ArithmeticOpcode(OpCodes.Sub, ArithmeticOperation.Subtraction);
		public static readonly ArithmeticOpcode Sub_Ovf = new ArithmeticOpcode(OpCodes.Sub_Ovf, ArithmeticOperation.Subtraction_OverflowCheck);
		public static readonly ArithmeticOpcode Sub_Ovf_Un = new ArithmeticOpcode(OpCodes.Sub_Ovf_Un, ArithmeticOperation.Subtraction_UnsignedWithOverflowCheck);
		public static readonly BranchOpcode Switch = new BranchOpcode(OpCodes.Switch);
		public static readonly CastOpcode Unbox = new CastOpcode(OpCodes.Unbox);
		public static readonly CastOpcode Unbox_Any = new CastOpcode(OpCodes.Unbox_Any);
		public static readonly ArithmeticOpcode Xor = new ArithmeticOpcode(OpCodes.Xor, ArithmeticOperation.BitwiseXor);
		#endregion

		#region Methods
		#region Misc Opcode Getters
		public static LoadConstantOpcode LoadInt32(int value)
		{
			switch (value)
			{
				case -1: return Ldc_I4_M1;
				case 0: return Ldc_I4_0;
				case 1: return Ldc_I4_1;
				case 2: return Ldc_I4_2;
				case 3: return Ldc_I4_3;
				case 4: return Ldc_I4_4;
				case 5: return Ldc_I4_5;
				case 6: return Ldc_I4_6;
				case 7: return Ldc_I4_7;
				case 8: return Ldc_I4_8;
				default: return (sbyte)value == value ? Ldc_I4_S : Ldc_I4;
			}
		}

		public static FieldReferenceOpcode FieldReference(LocationReferenceKind referenceKind, bool @static)
		{
			switch (referenceKind)
			{
				case LocationReferenceKind.Load: return @static ? Opcode.Ldsfld : Opcode.Ldfld;
				case LocationReferenceKind.LoadAddress: return @static ? Opcode.Ldsflda : Opcode.Ldflda;
				case LocationReferenceKind.Store: return @static ? Opcode.Stsfld : Opcode.Stfld;
				default: throw new ArgumentException("referenceKind");
			}
		}

		public static CallOpcode GetCall(CallKind kind)
		{
			switch (kind)
			{
				case CallKind.EarlyBound: return Call;
				case CallKind.Virtual: return Callvirt;
				case CallKind.Indirect: return Calli;
				case CallKind.Constructor: return Newobj;
				case CallKind.Jump: return Jmp;
				default: throw new ArgumentException("kind");
			}
		}

		public static ComparisonOpcode Compare(Comparison comparison)
		{
			switch (comparison)
			{
				case Comparison.Equal: return Ceq;
				case Comparison.GreaterThan: return Cgt;
				case Comparison.GreaterThan_Unsigned: return Cgt_Un;
				case Comparison.LessThan: return Clt;
				case Comparison.LessThan_Unsigned: return Clt_Un;
				default: return null;
			}
		}

		public static IndirectReferenceOpcode LoadIndirect(DataType type)
		{
			switch (type)
			{
				case DataType.Int8: return Ldind_I1;
				case DataType.Int16: return Ldind_I2;
				case DataType.Int32: return Ldind_I4;
				case DataType.Int64: return Ldind_I8;
				case DataType.NativeInt: return Ldind_I;
				case DataType.UInt8: return Ldind_U1;
				case DataType.UInt16: return Ldind_U2;
				case DataType.UInt32: return Ldind_U4;
				case DataType.UInt64: return Ldind_I8;
				case DataType.NativeUInt: return Ldind_I;
				case DataType.Float32: return Ldind_R4;
				case DataType.Float64: return Ldind_R8;
				case DataType.ObjectReference: return Ldind_Ref;
				default: return null;
			}
		}

		public static IndirectReferenceOpcode StoreIndirect(DataType type)
		{
			switch (type)
			{
				case DataType.Int8: return Stind_I1;
				case DataType.Int16: return Stind_I2;
				case DataType.Int32: return Stind_I4;
				case DataType.Int64: return Stind_I8;
				case DataType.NativeInt: return Stind_I;
				case DataType.UInt8: return Stind_I1;
				case DataType.UInt16: return Stind_I2;
				case DataType.UInt32: return Stind_I4;
				case DataType.UInt64: return Stind_I8;
				case DataType.NativeUInt: return Stind_I;
				case DataType.Float32: return Stind_R4;
				case DataType.Float64: return Stind_R8;
				case DataType.ObjectReference: return Stind_Ref;
				default: return null;
			}
		}
		#endregion

		#region Variable References
		public static VariableReferenceOpcode LoadArgument(int index)
		{
			Contract.Requires(index >= 0 && index < 0xFFFF);
			switch (index)
			{
				case 0: return Ldarg_0;
				case 1: return Ldarg_1;
				case 2: return Ldarg_2;
				case 3: return Ldarg_3;
				default: return index < 0x100 ? Ldarg_S : Ldarg;
			}
		}

		public static VariableReferenceOpcode LoadArgumentAddress(int index)
		{
			Contract.Requires(index >= 0 && index < 0xFFFF);
			return index < 0x100 ? Ldarga_S : Ldarga;
		}

		public static VariableReferenceOpcode StoreArgument(int index)
		{
			Contract.Requires(index >= 0 && index < 0xFFFF);
			return index < 0x100 ? Starg_S : Starg;
		}

		public static VariableReferenceOpcode LoadLocal(int index)
		{
			Contract.Requires(index >= 0 && index < 0xFFFF);
			switch (index)
			{
				case 0: return Ldloc_0;
				case 1: return Ldloc_1;
				case 2: return Ldloc_2;
				case 3: return Ldloc_3;
				default: return index < 0x100 ? Ldloc_S : Ldloc;
			}
		}

		public static VariableReferenceOpcode LoadLocalAddress(int index)
		{
			Contract.Requires(index >= 0 && index < 0xFFFF);
			return index < 0x100 ? Ldloca_S : Ldloca;
		}

		public static VariableReferenceOpcode StoreLocal(int index)
		{
			Contract.Requires(index >= 0 && index < 0xFFFF);
			return index < 0x100 ? Stloc_S : Stloc;
		}

		public static VariableReferenceOpcode Load(VariableLocation location)
		{
			return location.IsLocal ? LoadLocal(location.Index) : LoadArgument(location.Index);
		}

		public static VariableReferenceOpcode LoadAddress(VariableLocation location)
		{
			return location.IsLocal ? LoadLocalAddress(location.Index) : LoadArgumentAddress(location.Index);
		}

		public static VariableReferenceOpcode Store(VariableLocation location)
		{
			return location.IsLocal ? StoreLocal(location.Index) : StoreArgument(location.Index);
		}

		public static VariableReferenceOpcode LocalReference(LocationReferenceKind referenceKind, int index)
		{
			switch (referenceKind)
			{
				case LocationReferenceKind.Load: return LoadLocal(index);
				case LocationReferenceKind.LoadAddress: return LoadLocalAddress(index);
				case LocationReferenceKind.Store: return StoreLocal(index);
				default: throw new ArgumentException("referenceKind");
			}
		}

		public static VariableReferenceOpcode ArgumentReference(LocationReferenceKind referenceKind, int index)
		{
			switch (referenceKind)
			{
				case LocationReferenceKind.Load: return LoadArgument(index);
				case LocationReferenceKind.LoadAddress: return LoadArgumentAddress(index);
				case LocationReferenceKind.Store: return StoreArgument(index);
				default: throw new ArgumentException("referenceKind");
			}
		}

		public static VariableReferenceOpcode VariableReference(LocationReferenceKind referenceKind, VariableLocation location)
		{
			return location.IsLocal ? LocalReference(referenceKind, location.Index) : ArgumentReference(referenceKind, location.Index);
		}
		#endregion

		#region Branches
		public static BranchOpcode Branch(Comparison comparison, bool longForm)
		{
			switch (comparison)
			{
				case Comparison.Equal: return longForm ? Beq : Beq_S;
				case Comparison.NotEqual_Unordered: return longForm ? Bne_Un : Bne_Un_S;
				case Comparison.GreaterThanOrEqual: return longForm ? Bge : Bge_S;
				case Comparison.GreaterThanOrEqual_Unsigned: return longForm ? Bge_Un : Bge_Un_S;
				case Comparison.GreaterThan: return longForm ? Bgt : Bgt_S;
				case Comparison.GreaterThan_Unsigned: return longForm ? Bgt_Un : Bgt_Un_S;
				case Comparison.LessThanOrEqual: return longForm ? Ble : Ble_S;
				case Comparison.LessThanOrEqual_Unsigned: return longForm ? Ble_Un : Ble_Un_S;
				case Comparison.LessThan: return longForm ? Blt : Blt_S;
				case Comparison.LessThan_Unsigned: return longForm ? Blt_Un : Blt_Un_S;
				default: return null;
			}
		}

		public static BranchOpcode Branch(bool condition, bool longForm)
		{
			return condition
				? (longForm ? Brtrue : Brtrue_S)
				: (longForm ? Brfalse : Brfalse_S);
		}

		public static BranchOpcode Branch(bool longForm)
		{
			return longForm ? Br : Br_S;
		}
		#endregion
		#endregion
	}
}
