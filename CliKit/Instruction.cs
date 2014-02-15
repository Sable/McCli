using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliKit
{
	/// <summary>
	/// Represents a CIL instruction.
	/// </summary>
	public struct Instruction
	{
		#region Fields
		private readonly Opcode opcode;
		private readonly Operand operand;
		#endregion

		#region Constructors
		public Instruction(Opcode opcode)
		{
			Contract.Requires(opcode != null && opcode.OperandSizeInBytes == 0);

			this.opcode = opcode;
			this.operand = Operand.None;
		}

		public Instruction(Opcode opcode, Operand operand)
		{
			Contract.Requires(opcode != null);

			// TODO: Check that the operand has a type supported by the opcode.
			this.opcode = opcode;
			this.operand = operand;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the opcode of this instruction.
		/// </summary>
		public Opcode Opcode
		{
			get { return opcode; }
		}

		/// <summary>
		/// Gets the inline operand in this instruction.
		/// </summary>
		public Operand Operand
		{
			get { return operand; }
		}
		#endregion
	}
}
