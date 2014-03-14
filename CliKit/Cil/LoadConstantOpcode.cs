using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emit = System.Reflection.Emit;

namespace CliKit.Cil
{
	/// <summary>
	/// Represents CIL opcodes which load constant values,
	/// including integral and float types, plus ldnull and ldstr.
	/// </summary>
	public sealed class LoadConstantOpcode : Opcode
	{
		#region Fields
		// sbyte.MinValue means undefined
		private readonly sbyte constantValue;
		#endregion

		#region Constructors
		internal LoadConstantOpcode(Emit.OpCode opcode) : base(opcode)
		{
			char lastChar = opcode.Name[opcode.Name.Length - 1];
			if (lastChar >= '0' && lastChar <= '9')
			{
				constantValue = (sbyte)(lastChar - '0');
				if (opcode.Name[opcode.Name.Length - 2] == 'M')
					constantValue *= -1;
			}
			else
			{
				constantValue = sbyte.MinValue;
			}
		}
		#endregion

		#region Properties
		public override OpcodeForm Form
		{
			get
			{
				switch (OperandKind)
				{
					case OperandKind.Int8: return OpcodeForm.Short;
					case OperandKind.None:
						if (opcode.StackBehaviourPush == Emit.StackBehaviour.Pushi) return OpcodeForm.Macro;
						break;
				}

				return OpcodeForm.Normal;
			}
		}

		public DataType DataType
		{
			get
			{
				switch (OperandKind)
				{
					case OperandKind.Int8: return DataType.Int8;
					case OperandKind.Int32: return DataType.Int32;
					case OperandKind.Int64: return DataType.Int64;
					case OperandKind.Float64: return DataType.Float64;
					case OperandKind.Float32: return DataType.Float32;
					case OperandKind.StringToken: return DataType.ObjectReference;
					case OperandKind.None: break;
					default: throw new NotSupportedException();
				}

				// No operand, can be an int or null
				switch (opcode.StackBehaviourPush)
				{
					case Emit.StackBehaviour.Pushi: return DataType.Int32;
					case Emit.StackBehaviour.Push1: return DataType.ObjectReference; // Ldnull
					default: throw new NotSupportedException();
				}
			}
		}

		public int? MacroIntValue
		{
			get { return constantValue == sbyte.MinValue ? (int?)null : (int)constantValue; }
		}
		#endregion

		#region Methods
		public override void Accept<T>(OpcodeVisitor<T> visitor, T param)
		{
			visitor.VisitLoadConstant(this, param);
		}
		#endregion
	}
}
