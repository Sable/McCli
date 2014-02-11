using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emit = System.Reflection.Emit;

namespace CliKit.Cil
{
	public sealed class BranchOpcode : Opcode
	{
		#region Fields
		private readonly Comparison comparison;
		#endregion

		#region Constructors
		internal BranchOpcode(Emit.OpCode opcode, Comparison comparison)
			: base(opcode)
		{
			this.comparison = comparison;
		}

		internal BranchOpcode(Emit.OpCode opcode) : base(opcode) {}
		#endregion

		#region Properties
		public int PopCount
		{
			get
			{
				switch (opcode.StackBehaviourPop)
				{
					case Emit.StackBehaviour.Pop0: return 0;
					case Emit.StackBehaviour.Pop1: return 1;
					case Emit.StackBehaviour.Pop1_pop1: return 2;
					default: throw new NotImplementedException();
				}
			}
		}

		public bool IsUnconditional
		{
			get { return opcode.StackBehaviourPop == Emit.StackBehaviour.Pop0; }
		}

		public bool IsConditional
		{
			get { return opcode.StackBehaviourPop != Emit.StackBehaviour.Pop0; }
		}

		public bool IsBoolean
		{
			get { return opcode.StackBehaviourPop == Emit.StackBehaviour.Pop1; }
		}

		public bool IsComparison
		{
			get { return opcode.StackBehaviourPop == Emit.StackBehaviour.Pop1_pop1; }
		}

		public bool Boolean
		{
			get
			{
				Contract.Requires(IsBoolean);
				return opcode.Name[2] == 't' ? true : false;
			}
		}

		public Comparison Comparison
		{
			get
			{
				Contract.Requires(IsComparison);
				return comparison;
			}
		}

		public bool IsShortForm
		{
			get { return opcode.OperandType == Emit.OperandType.ShortInlineBrTarget; }
		}
		#endregion
	}
}
