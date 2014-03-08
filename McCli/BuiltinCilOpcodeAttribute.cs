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
		private readonly int opcode;
		#endregion

		#region Constructors
		public BuiltinCilOpcodeAttribute(int opcode)
		{
			this.opcode = opcode;
		}
		#endregion

		#region Properties
		public int Opcode
		{
			get { return opcode; }
		}
		#endregion

		#region Methods
		#endregion
	}
}
