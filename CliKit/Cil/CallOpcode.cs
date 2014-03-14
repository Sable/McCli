using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emit = System.Reflection.Emit;

namespace CliKit.Cil
{
	/// <summary>
	/// Represents CLI opcodes performing calls (early bound, virtual, constructor or jump).
	/// </summary>
	public sealed class CallOpcode : Opcode
	{
		#region Constructors
		internal CallOpcode(Emit.OpCode opcode) : base(opcode) { }
		#endregion

		#region Properties
		public CallKind Kind
		{
			get
			{
				switch (Value)
				{
					case OpcodeValue.Call: return CallKind.EarlyBound;
					case OpcodeValue.Calli: return CallKind.Indirect;
					case OpcodeValue.Callvirt: return CallKind.Virtual;
					case OpcodeValue.Jmp: return CallKind.Jump;
					case OpcodeValue.Newobj: return CallKind.Constructor;
					default: throw new NotSupportedException();
				}
			}
		}

		public override PrefixMask ValidPrefixes
		{
			get
			{
				switch (Value)
				{
					case OpcodeValue.Call: return PrefixMask.Tail;
					case OpcodeValue.Calli: return PrefixMask.Tail;
					case OpcodeValue.Callvirt: return PrefixMask.SuppressCheck | PrefixMask.Tail;
					case OpcodeValue.Jmp: return PrefixMask.SuppressCheck;
					case OpcodeValue.Newobj: return PrefixMask.None;
					default: throw new NotSupportedException();
				}
				throw new NotImplementedException();
			}
		}
		#endregion

		#region Methods
		public override void Accept<T>(OpcodeVisitor<T> visitor, T param)
		{
			visitor.VisitCall(this, param);
		}
		#endregion
	}
}
