using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McCli
{
	/// <summary>
	/// Marks a MatLab builtin function as being equivalent to a CIL opcode.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public sealed class BuiltinCilOpcodeAttribute : Attribute
	{
		#region Fields
		private readonly ushort[] opcodes;
		#endregion

		#region Constructors
		public BuiltinCilOpcodeAttribute(params ushort[] opcodes)
		{
			this.opcodes = opcodes;
		}
		#endregion

		#region Properties
		public IList<ushort> Opcodes
		{
			get { return opcodes; }
		}
		#endregion

		#region Methods
		#endregion
	}
}
