using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliKit
{
	/// <summary>
	/// Specifies the way a memory location is referenced.
	/// </summary>
	public enum LocationReferenceKind
	{
		/// <summary>
		/// Indicates that the value at the location is loaded on the evaluation stack.
		/// </summary>
		Load,

		/// <summary>
		/// Indicates that the address of the location is loaded on the evaluation stack.
		/// </summary>
		LoadAddress,

		/// <summary>
		/// Indicates that a value from the evaluation stack is stored at the location.
		/// </summary>
		Store
	}
}
