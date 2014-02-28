using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emit = System.Reflection.Emit;

namespace CliKit
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
				switch (opcode.Value)
				{
					case 0x28: return CallKind.EarlyBound;
					case 0x29: return CallKind.Indirect;
					case 0x6F: return CallKind.Virtual;
					case 0x27: return CallKind.Jump;
					case 0x73: return CallKind.Constructor;
					default: throw new InvalidOperationException();
				}
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
