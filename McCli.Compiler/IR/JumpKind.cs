using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compiler.IR
{
	/// <summary>
	/// Identifies a kind of jump statement.
	/// </summary>
	public enum JumpKind
	{
		/// <summary>
		/// Indicates a return statement, which exits the function.
		/// </summary>
		Return,

		/// <summary>
		/// Indicates a continue statement, which repeats the innermost loop.
		/// </summary>
		Continue,

		/// <summary>
		/// Indicates a break statement, which exits the innermost loop.
		/// </summary>
		Break
	}
}
