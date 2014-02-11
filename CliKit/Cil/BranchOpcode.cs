using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emit = System.Reflection.Emit;

namespace CliKit.Cil
{
	public sealed class BranchOpcode : Opcode
	{
		#region Constructors
		internal BranchOpcode(Emit.OpCode opcode) : base(opcode) { }
		#endregion

		#region Properties
		public bool IsConditional
		{
			get { return opcode.StackBehaviourPop == Emit.StackBehaviour.Popi; }
		}

		public bool? Condition
		{
			get
			{
				if (opcode.StackBehaviourPop == Emit.StackBehaviour.Popi) return null;
				return opcode.Name[2] == 't' ? true : false;
			}
		}

		public bool IsShortForm
		{
			get { return opcode.OperandType == Emit.OperandType.ShortInlineBrTarget; }
		}
		#endregion
	}
}
