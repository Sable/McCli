using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emit = System.Reflection.Emit;

namespace CliKit
{
	/// <summary>
	/// Represents opcodes which branch conditionally or not.
	/// </summary>
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
		public BranchKind BranchKind
		{
			get
			{
				switch (opcode.StackBehaviourPop)
				{
					case Emit.StackBehaviour.Pop0: return CliKit.BranchKind.Unconditional;
					case Emit.StackBehaviour.Popi: return CliKit.BranchKind.Boolean;
					case Emit.StackBehaviour.Pop1_pop1: return CliKit.BranchKind.Comparison;
					default: throw new NotImplementedException();
				}
			}
		}

		public bool Boolean
		{
			get
			{
				Contract.Requires(BranchKind == CliKit.BranchKind.Boolean);
				return opcode.Name[2] == 't' ? true : false;
			}
		}

		public Comparison Comparison
		{
			get
			{
				Contract.Requires(BranchKind == CliKit.BranchKind.Comparison);
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
