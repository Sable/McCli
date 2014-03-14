using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emit = System.Reflection.Emit;

namespace CliKit.Cil
{
	/// <summary>
	/// Represents CIL opcodes for type casting and testing, boxing and unboxing.
	/// All opcodes of this class take type tokens as inline operands.
	/// </summary>
	public sealed class CastOpcode : Opcode
	{
		#region Constructors
		internal CastOpcode(Emit.OpCode opcode) : base(opcode) { }
		#endregion

		#region Properties
		public CastKind Kind
		{
			get
			{
				switch (Value)
				{
					case OpcodeValue.Castclass: return CastKind.Class;
					case OpcodeValue.Box: return CastKind.Box;
					case OpcodeValue.Unbox: return CastKind.Unbox;
					case OpcodeValue.Unbox_Any: return CastKind.Unbox;
					case OpcodeValue.Isinst: return CastKind.Test;
					default: throw new NotImplementedException();
				}
			}
		}

		public override PrefixMask ValidPrefixes
		{
			get
			{
				switch (Value)
				{
					case OpcodeValue.Castclass:
					case OpcodeValue.Unbox:
						return PrefixMask.SuppressCheck;

					default:
						return PrefixMask.None;
				}
			}
		}
		#endregion

		#region Methods
		public override void Accept<T>(OpcodeVisitor<T> visitor, T param)
		{
			visitor.VisitCast(this, param);
		}
		#endregion
	}
}
