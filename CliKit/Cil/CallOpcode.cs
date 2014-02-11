using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emit = System.Reflection.Emit;

namespace CliKit.Cil
{
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
					default: throw new InvalidOperationException();
				}
			}
		}
		#endregion
	}
}
