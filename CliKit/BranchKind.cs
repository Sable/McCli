using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliKit
{
	/// <summary>
	/// Identifies a kind of branching operation.
	/// </summary>
	public enum BranchKind
	{
		/// <summary>
		/// Indicates an unconditional branch.
		/// </summary>
		Unconditional,

		/// <summary>
		/// Indicates a branch out of an exception handling block.
		/// </summary>
		Leave,

		/// <summary>
		/// Indicates a branch conditional on a single value.
		/// </summary>
		Boolean,

		/// <summary>
		/// Indicates a branch conditional on the result of the comparison of two values.
		/// </summary>
		Comparison,

		/// <summary>
		/// Indicates a branch whose destination is indexed by some value.
		/// </summary>
		Switch
	}
}
