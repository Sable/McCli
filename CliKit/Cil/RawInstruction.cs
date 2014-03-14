using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliKit.Cil
{
	/// <summary>
	/// Represents a CIL instruction (opcode and optional operand) without symbolic information.
	/// </summary>
	public struct RawInstruction
	{
		#region Fields
		/// <summary>
		/// The <see cref="Opcode"/> instance or, exceptionally for the <c>switch</c> opcode,
		/// the array of switch branch offsets.
		/// </summary>
		private readonly object opcodeOrJumpTable;
		private readonly NumericalOperand numericalOperand;
		#endregion

		#region Constructors
		public RawInstruction(Opcode opcode)
		{
			Contract.Requires(opcode != null && opcode.OperandKind == OperandKind.None);

			this.opcodeOrJumpTable = opcode;
			this.numericalOperand = default(NumericalOperand);
		}

		public RawInstruction(Opcode opcode, NumericalOperand operand)
		{
			Contract.Requires(opcode != null && opcode != Opcode.Switch);

			this.opcodeOrJumpTable = opcode;
			this.numericalOperand = operand;
		}

		private RawInstruction(int[] switchJumpTable)
		{
			this.opcodeOrJumpTable = switchJumpTable;
			this.numericalOperand = default(NumericalOperand);
		}
		#endregion

		#region Properties
		public Opcode Opcode
		{
			get
			{
				var opcode = opcodeOrJumpTable as Opcode;
				if (opcode != null) return opcode;
				if (opcodeOrJumpTable is int[]) return Opcode.Switch;
				return Opcode.Nop;
			}
		}

		public OpcodeValue OpcodeValue
		{
			get { return Opcode.Value; }
		}

		public bool IsSwitch
		{
			get { return opcodeOrJumpTable is int[]; }
		}

		public NumericalOperand NumericalOperand
		{
			get { return numericalOperand; }
		}

		public int IntOperand
		{
			get { return numericalOperand.Int; }
		}

		public uint UIntOperand
		{
			get { return numericalOperand.UInt; }
		}

		public long Int64Operand
		{
			get { return numericalOperand.Int64; }
		}

		public float Float32Operand
		{
			get { return numericalOperand.Float32; }
		}

		public double Float64Operand
		{
			get { return numericalOperand.Float64; }
		}

		public MetadataToken MetadataTokenOperand
		{
			get { return numericalOperand.MetadataToken; }
		}

		/// <summary>
		/// For switch instructions, gets the associated jump table.
		/// </summary>
		public int[] JumpTable
		{
			get
			{
				Contract.Requires(IsSwitch);
				return (int[])opcodeOrJumpTable;
			}
		}

		public int SizeInBytes
		{
			get
			{
				var opcode = opcodeOrJumpTable as Opcode;
				if (opcode != null) return (int)opcode.InstructionSizeInBytes;
				
				var jumpTable = opcodeOrJumpTable as int[];
				if (jumpTable != null) return 5 + jumpTable.Length * 4;
				
				Contract.Assert(opcodeOrJumpTable == null);
				return 1; // Nop
			}
		}
		#endregion

		#region Methods
		public static RawInstruction CreateSwitch(int[] jumpTable)
		{
			Contract.Requires(jumpTable != null);
			return new RawInstruction(jumpTable);
		}

		public override string ToString()
		{
			return Opcode.Name;
		}
		#endregion
	}
}
