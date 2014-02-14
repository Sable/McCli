using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliKit
{
	public struct Instruction
	{
		#region Fields
		private readonly Opcode opcode;
		private readonly Operand operand;
		#endregion

		#region Constructors
		public Instruction(Opcode opcode)
		{
			Contract.Requires(opcode != null && opcode.OperandSizeInBytes != 0);

			this.opcode = opcode;
			this.operand = Operand.None;
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

		public Operand Operand
		{
			get { return operand; }
		}
		#endregion
	}
}
