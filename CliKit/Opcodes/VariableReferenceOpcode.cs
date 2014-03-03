using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emit = System.Reflection.Emit;

namespace CliKit
{
	/// <summary>
	/// Represents CIL opcodes that refers to a local variable or argument. <c>[ls]darg(\.[0123s])?</c>
	/// </summary>
	public sealed class VariableReferenceOpcode : Opcode
	{
		#region Constructors
		internal VariableReferenceOpcode(Emit.OpCode opcode) : base(opcode) {}
		#endregion

		#region Properties
		public VariableKind VariableKind
		{
			get { return opcode.Name[2] == 'a' ? VariableKind.Argument : VariableKind.Local; }
		}

		public LocationReferenceKind ReferenceKind
		{
			get
			{
				var name = Name;
				if (name[0] == 's') return LocationReferenceKind.Store;
				if (name.Length >= 6 && name[5] == 'a') return LocationReferenceKind.LoadAddress;
				return LocationReferenceKind.Load;
			}
		}

		public bool IsParameter
		{
			get { return VariableKind == VariableKind.Argument; }
		}

		public bool IsLocal
		{
			get { return VariableKind == VariableKind.Local; }
		}

		public bool IsLoad
		{
			get { return ReferenceKind == LocationReferenceKind.Load; }
		}

		public bool IsLoadAddress
		{
			get { return ReferenceKind == LocationReferenceKind.LoadAddress; }
		}

		public bool IsStore
		{
			get { return ReferenceKind == LocationReferenceKind.Store; }
		}

		public bool IsLongForm
		{
			get { return OperandType == Emit.OperandType.InlineVar; }
		}

		public bool IsShortForm
		{
			get { return OperandType == Emit.OperandType.ShortInlineVar; }
		}

		public bool IsConstantForm
		{
			get { return OperandType == Emit.OperandType.InlineNone; }
		}

		public int? ConstantIndex
		{
			get
			{
				char lastChar = opcode.Name[opcode.Name.Length - 1];
				return lastChar >= '0' && lastChar < '9' ? (lastChar - '0') : (int?)null;
			}
		}
		#endregion

		#region Methods
		public override void Accept<T>(OpcodeVisitor<T> visitor, T param)
		{
			visitor.VisitVariableReference(this, param);
		}
		#endregion
	}
}
