﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emit = System.Reflection.Emit;

namespace CliKit
{
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
		public DataTypes ConstantDataType
		{
			get
			{
				switch (opcode.OperandType)
				{
					case Emit.OperandType.ShortInlineI: return DataTypes.Int8;
					case Emit.OperandType.InlineI: return DataTypes.Int32;
					case Emit.OperandType.InlineI8: return DataTypes.Int64;
					case Emit.OperandType.InlineR: return DataTypes.Float64;
					case Emit.OperandType.ShortInlineR: return DataTypes.Float32;
					case Emit.OperandType.InlineString: return DataTypes.ObjectReference;
					case Emit.OperandType.InlineNone: break;
					default: throw new NotSupportedException();
				}

				// No operand, can be an int or null
				switch (opcode.StackBehaviourPush)
				{
					case Emit.StackBehaviour.Pushi: return DataTypes.Int32;
					case Emit.StackBehaviour.Push1: return DataTypes.ObjectReference; // Ldnull
					default: throw new NotSupportedException();
				}
			}
		}

		public int? ConstantValue
		{
			get { return constantValue == sbyte.MinValue ? (int?)null : (int)constantValue; }
		}
		#endregion

		#region Methods
		#endregion
	}
}