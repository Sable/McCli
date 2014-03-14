using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliKit.Cil
{
	/// <summary>
	/// Identifies a kind of casting opcode.
	/// </summary>
	public enum CastKind
	{
		/// <summary>
		/// Indicates up or downcasting between reference types.
		/// </summary>
		Class,

		/// <summary>
		/// Indicates the conversion a value type from its unboxed to its boxed form.
		/// </summary>
		Box,

		/// <summary>
		/// Indicates the conversion a value type from its boxed to its unboxed form.
		/// </summary>
		Unbox,

		/// <summary>
		/// Indicates a type test returning a boolean value.
		/// </summary>
		Test
	}
}
