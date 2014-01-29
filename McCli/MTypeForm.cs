using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// Identifies the runtime representation of a type.
	/// </summary>
	[Flags]
	public enum MTypeForm
	{
		/// <summary>
		/// Indicates an array representation (ie the default for numerical types).
		/// </summary>
		Array = 0,

		/// <summary>
		/// Indicates a scalar representation.
		/// </summary>
		Scalar = 1,

		/// <summary>
		/// Indicates a sparse matrix representation.
		/// </summary>
		SparseMatrix = 2
	}
}
