using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emit = System.Reflection.Emit;

namespace CliKit
{
	/// <summary>
	/// Represents an opcode which references the value pointed to by a pointer.
	/// </summary>
	public sealed class IndirectReferenceOpcode : Opcode
	{
		#region Constructors
		internal IndirectReferenceOpcode(Emit.OpCode opcode) : base(opcode) { }
		#endregion

		#region Properties
		public LocationReferenceKind ReferenceKind
		{
			get
			{
				return opcode.StackBehaviourPush == Emit.StackBehaviour.Push0
					? LocationReferenceKind.Store : LocationReferenceKind.Load;
			}
		}

		public bool IsLoad
		{
			get { return opcode.StackBehaviourPush != Emit.StackBehaviour.Push0; }
		}

		public bool IsStore
		{
			get { return opcode.StackBehaviourPush == Emit.StackBehaviour.Push0; }
		}

		public bool HasTypeOperand
		{
			get { return opcode.OperandType == Emit.OperandType.InlineType; }
		}

		public DataType? DataType
		{
			get
			{
				var name = opcode.Name;
				int dotIndex = name.IndexOf('.');
				if (dotIndex < 0) return null; // ldobj, stobj

				var typeName = name.Substring(dotIndex + 1);
				return DataTypeEnum.TryParseNameInOpcode(typeName);
			}
		}
		#endregion

		#region Methods
		public override void Accept<T>(OpcodeVisitor<T> visitor, T param)
		{
			visitor.VisitIndirectReference(this, param);
		}
		#endregion
	}
}
