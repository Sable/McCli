
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
					case Emit.StackBehaviour.Pop0: return BranchKind.Unconditional;
					case Emit.StackBehaviour.Popi: return BranchKind.Boolean;
					case Emit.StackBehaviour.Pop1_pop1: return BranchKind.Comparison;
					default: break;
				}

				if (opcode.Value == 0x45) return BranchKind.Switch;
				if (opcode.Value == 0xDD || opcode.Value == 0xDE) return BranchKind.Leave;
				throw new NotImplementedException();
			}
		}

		public bool Boolean
		{
			get
			{
				Contract.Requires(BranchKind == BranchKind.Boolean);
				return opcode.Name[2] == 't' ? true : false;
			}
		}

		public Comparison Comparison
		{
			get
			{
				Contract.Requires(BranchKind == BranchKind.Comparison);
				return comparison;
			}
		}

		public override OpcodeForm Form
		{
			get { return OperandKind == OperandKind.BranchTarget8 ? OpcodeForm.Short : OpcodeForm.Normal; }
		}
		#endregion

		#region Methods
		public override void Accept<T>(OpcodeVisitor<T> visitor, T param)
		{
			visitor.VisitBranch(this, param);
		}
		#endregion
	}
}
