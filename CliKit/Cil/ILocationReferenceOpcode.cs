using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliKit.Cil
{
	/// <summary>
	/// Interface for opcodes which reference a storage location.
	/// </summary>
	public interface ILocationReferenceOpcode
	{
		#region Properties
		/// <summary>
		/// Gets the kind of storage location being referenced by this opcode.
		/// </summary>
		LocationKind LocationKind { get; }

		/// <summary>
		/// Gets the way the location is being referenced by this opcode.
		/// </summary>
		LocationReferenceKind ReferenceKind { get; }
		#endregion
	}
}
