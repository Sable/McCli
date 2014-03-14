using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliKit
{
	/// <summary>
	/// Specifies a form of an operand, for example distinguishing ldc.i4, ldc.i4.s and ldc.i4.0.
	/// </summary>
	public enum OpcodeForm
	{
		/// <summary>
		/// Indicates the normal form of an opcode, with any operand explicit and full-sized.
		/// For example, <c>ldc.i4</c>.
		/// </summary>
		Normal,

		/// <summary>
		/// Indicates the short form of an opcode, with an explicit operand that is not full-sized.
		/// For example, <c>ldc.i4.s</c>.
		/// </summary>
		Short,

		/// <summary>
		/// Indicates the macro form of an opcode, with an implicit constant-valued operand.
		/// For example, <c>ldc.i4.0</c>.
		/// </summary>
		Macro
	}
}
