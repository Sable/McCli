using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliKit
{
	/// <summary>
	/// Specifies one or more prefix opcodes.
	/// </summary>
	[Flags]
	public enum PrefixMask
	{
		/// <summary>
		/// Indicate the absence of prefixes.
		/// </summary>
		None = 0,

		/// <summary>
		/// Indicates the suppress check (AKA 'no') prefix, which disables a fault check.
		/// </summary>
		SuppressCheck = 1 << 0,

		/// <summary>
		/// Indicates the constrained prefix, which is used for virtual calls of value type methods.
		/// </summary>
		Constrained = 1 << 1,

		/// <summary>
		/// Indicates the readonly prefix, which allows loading the address of an array element
		/// for read-only purposes, without performing a type check.
		/// </summary>
		Readonly = 1 << 2,

		/// <summary>
		/// Indicates the tail prefix, which marks a call as being a tail call.
		/// </summary>
		Tail = 1 << 3,

		/// <summary>
		/// Indicates the unaligned prefix, which marks the next instruction as accessing potentially unaligned memory.
		/// </summary>
		Unaligned = 1 << 4,

		/// <summary>
		/// Indicates the volatile prefix, which prohibits certain optimizations on a memory access.
		/// </summary>
		Volatile = 1 << 5
	}
}
