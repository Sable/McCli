using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emit = System.Reflection.Emit;

namespace CliKit.Cil
{
	/// <summary>
	/// Abstract base class for CLI opcodes.
	/// Instances are immutable and reference comparable.
	/// </summary>
	public class Opcode
	{
		#region Fields
		// TODO: Support ret, add, castclass
		public static readonly BranchOpcode Br = new BranchOpcode(Emit.OpCodes.Br);
		public static readonly BranchOpcode Br_S = new BranchOpcode(Emit.OpCodes.Br_S);
		public static readonly BranchOpcode Brfalse = new BranchOpcode(Emit.OpCodes.Brfalse);
		public static readonly BranchOpcode Brfalse_S = new BranchOpcode(Emit.OpCodes.Brfalse_S);
		public static readonly BranchOpcode Brtrue = new BranchOpcode(Emit.OpCodes.Brtrue);
		public static readonly BranchOpcode Brtrue_S = new BranchOpcode(Emit.OpCodes.Brtrue_S);
		public static readonly CallOpcode Call = new CallOpcode(Emit.OpCodes.Call);
		public static readonly CallOpcode Calli = new CallOpcode(Emit.OpCodes.Calli);
		public static readonly CallOpcode Callvirt = new CallOpcode(Emit.OpCodes.Callvirt);
		public static readonly Opcode Castclass = new Opcode(Emit.OpCodes.Castclass);
		public static readonly ComparisonOpcode Ceq = new ComparisonOpcode(Emit.OpCodes.Ceq, Comparison.Equal);
		public static readonly ComparisonOpcode Cgt = new ComparisonOpcode(Emit.OpCodes.Cgt, Comparison.GreaterThan);
		public static readonly ComparisonOpcode Cgt_Un = new ComparisonOpcode(Emit.OpCodes.Cgt_Un, Comparison.GreaterThan_Unsigned);
		public static readonly ComparisonOpcode Clt = new ComparisonOpcode(Emit.OpCodes.Clt, Comparison.LessThan);
		public static readonly ComparisonOpcode Clt_Un = new ComparisonOpcode(Emit.OpCodes.Clt_Un, Comparison.LessThan_Unsigned);
		public static readonly VariableReferenceOpcode Ldarg = new VariableReferenceOpcode(Emit.OpCodes.Ldarg);
		public static readonly VariableReferenceOpcode Ldarg_0 = new VariableReferenceOpcode(Emit.OpCodes.Ldarg_0);
		public static readonly VariableReferenceOpcode Ldarg_1 = new VariableReferenceOpcode(Emit.OpCodes.Ldarg_1);
		public static readonly VariableReferenceOpcode Ldarg_2 = new VariableReferenceOpcode(Emit.OpCodes.Ldarg_2);
		public static readonly VariableReferenceOpcode Ldarg_3 = new VariableReferenceOpcode(Emit.OpCodes.Ldarg_3);
		public static readonly VariableReferenceOpcode Ldarg_S = new VariableReferenceOpcode(Emit.OpCodes.Ldarg_S);
		public static readonly VariableReferenceOpcode Ldarga = new VariableReferenceOpcode(Emit.OpCodes.Ldarga);
		public static readonly VariableReferenceOpcode Ldarga_S = new VariableReferenceOpcode(Emit.OpCodes.Ldarga_S);
		public static readonly LoadConstantOpcode Ldc_I4 = new LoadConstantOpcode(Emit.OpCodes.Ldc_I4);
		public static readonly LoadConstantOpcode Ldc_I4_0 = new LoadConstantOpcode(Emit.OpCodes.Ldc_I4_0);
		public static readonly LoadConstantOpcode Ldc_I4_1 = new LoadConstantOpcode(Emit.OpCodes.Ldc_I4_1);
		public static readonly LoadConstantOpcode Ldc_I4_2 = new LoadConstantOpcode(Emit.OpCodes.Ldc_I4_2);
		public static readonly LoadConstantOpcode Ldc_I4_3 = new LoadConstantOpcode(Emit.OpCodes.Ldc_I4_3);
		public static readonly LoadConstantOpcode Ldc_I4_4 = new LoadConstantOpcode(Emit.OpCodes.Ldc_I4_4);
		public static readonly LoadConstantOpcode Ldc_I4_5 = new LoadConstantOpcode(Emit.OpCodes.Ldc_I4_5);
		public static readonly LoadConstantOpcode Ldc_I4_6 = new LoadConstantOpcode(Emit.OpCodes.Ldc_I4_6);
		public static readonly LoadConstantOpcode Ldc_I4_7 = new LoadConstantOpcode(Emit.OpCodes.Ldc_I4_7);
		public static readonly LoadConstantOpcode Ldc_I4_8 = new LoadConstantOpcode(Emit.OpCodes.Ldc_I4_8);
		public static readonly LoadConstantOpcode Ldc_I4_M1 = new LoadConstantOpcode(Emit.OpCodes.Ldc_I4_M1);
		public static readonly LoadConstantOpcode Ldc_I4_S = new LoadConstantOpcode(Emit.OpCodes.Ldc_I4_S);
		public static readonly LoadConstantOpcode Ldc_I8 = new LoadConstantOpcode(Emit.OpCodes.Ldc_I8);
		public static readonly LoadConstantOpcode Ldc_R4 = new LoadConstantOpcode(Emit.OpCodes.Ldc_R4);
		public static readonly LoadConstantOpcode Ldc_R8 = new LoadConstantOpcode(Emit.OpCodes.Ldc_R8);
		public static readonly FieldReferenceOpcode Ldfld = new FieldReferenceOpcode(Emit.OpCodes.Ldfld);
		public static readonly FieldReferenceOpcode Ldflda = new FieldReferenceOpcode(Emit.OpCodes.Ldflda);
		public static readonly VariableReferenceOpcode Ldloc = new VariableReferenceOpcode(Emit.OpCodes.Ldloc);
		public static readonly VariableReferenceOpcode Ldloc_0 = new VariableReferenceOpcode(Emit.OpCodes.Ldloc_0);
		public static readonly VariableReferenceOpcode Ldloc_1 = new VariableReferenceOpcode(Emit.OpCodes.Ldloc_1);
		public static readonly VariableReferenceOpcode Ldloc_2 = new VariableReferenceOpcode(Emit.OpCodes.Ldloc_2);
		public static readonly VariableReferenceOpcode Ldloc_3 = new VariableReferenceOpcode(Emit.OpCodes.Ldloc_3);
		public static readonly VariableReferenceOpcode Ldloc_S = new VariableReferenceOpcode(Emit.OpCodes.Ldloc_S);
		public static readonly VariableReferenceOpcode Ldloca = new VariableReferenceOpcode(Emit.OpCodes.Ldloca);
		public static readonly VariableReferenceOpcode Ldloca_S = new VariableReferenceOpcode(Emit.OpCodes.Ldloca_S);
		public static readonly FieldReferenceOpcode Ldsfld = new FieldReferenceOpcode(Emit.OpCodes.Ldsfld);
		public static readonly FieldReferenceOpcode Ldsflda = new FieldReferenceOpcode(Emit.OpCodes.Ldsflda);
		public static readonly Opcode Ret = new Opcode(Emit.OpCodes.Ret);
		public static readonly VariableReferenceOpcode Starg = new VariableReferenceOpcode(Emit.OpCodes.Starg);
		public static readonly VariableReferenceOpcode Starg_S = new VariableReferenceOpcode(Emit.OpCodes.Starg_S);
		public static readonly FieldReferenceOpcode Stfld = new FieldReferenceOpcode(Emit.OpCodes.Stfld);
		public static readonly VariableReferenceOpcode Stloc = new VariableReferenceOpcode(Emit.OpCodes.Stloc);
		public static readonly VariableReferenceOpcode Stloc_S = new VariableReferenceOpcode(Emit.OpCodes.Stloc_S);
		public static readonly FieldReferenceOpcode Stsfld = new FieldReferenceOpcode(Emit.OpCodes.Stsfld);

		protected Emit.OpCode opcode;
		#endregion

		#region Constructors
		internal Opcode(Emit.OpCode opcode)
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

		public byte FirstCodeByte
		{
			get { return unchecked((byte)Code); }
		}

		public bool HasTwoCodeBytes
		{
			get { return IsFirstOfTwoBytes(FirstCodeByte); }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Gets the <see cref="System.Reflection.Emit.OpCode"/> for this opcode.
		/// </summary>
		/// <param name="opCode">The BCL opcode.</param>
		public void GetEmitOpCode(out Emit.OpCode opCode)
		{
			opCode = this.opcode;
		}

		public override string ToString()
		{
			return Name;
		}

		public static bool IsFirstOfTwoBytes(byte @byte)
		{
			return @byte == 0xFE;
		}
		#endregion
	}
}
