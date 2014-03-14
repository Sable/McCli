using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliKit.Cil
{
	/// <summary>
	/// Identifies a kind of storage location.
	/// </summary>
	public enum LocationKind
	{
		/// <summary>
		/// Indicates an argument's storage location.
		/// </summary>
		Argument,

		/// <summary>
		/// Indicates a local variable storage location.
		/// </summary>
		Local,

		/// <summary>
		/// Indicates an instance or static field of an object or type.
		/// </summary>
		Field,

		/// <summary>
		/// Indicates the storage location associated with an array element.
		/// </summary>
		ArrayElement,

		/// <summary>
		/// Indicates an indeterminate storage location referenced by a pointer.
		/// </summary>
		Pointee
	}

	public static class LocationKindEnum
	{
		public static bool IsVariable(this LocationKind kind)
		{
			return kind == LocationKind.Argument || kind == LocationKind.Local;
		}
	}
}
