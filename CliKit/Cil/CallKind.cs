using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliKit.Cil
{
	/// <summary>
	/// Identifies a type of function call made by an instruction.
	/// </summary>
	public enum CallKind
	{
		/// <summary>
		/// Indicates a call to a method known at compile-time and without virtual behavior.
		/// </summary>
		EarlyBound,

		/// <summary>
		/// Indicates a call to an instance method which has virtual behavior if the method is virtual.
		/// </summary>
		Virtual,

		/// <summary>
		/// Indicates a call through a function pointer.
		/// </summary>
		Indirect,

		/// <summary>
		/// Indicates a transfer of control to another method.
		/// </summary>
		Jump,

		/// <summary>
		/// Indicates a call to a constructor.
		/// </summary>
		Constructor
	}
}
