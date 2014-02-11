using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emit = System.Reflection.Emit;

namespace CliKit.Cil
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
		public StackEntryKind Kind
		{
			get
			{
				if (opcode.Name[4] == 'R') return StackEntryKind.FloatingPoint;
				return opcode.Name[5] == '4' ? StackEntryKind.Int32 : StackEntryKind.Int64;
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
