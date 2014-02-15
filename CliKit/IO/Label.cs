using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliKit.IO
{
	/// <summary>
	/// Abstract base class for CIL labels, used as branch targets.
	/// </summary>
	public abstract class Label
	{
		#region Properties
		/// <summary>
		/// Gets the name of this label for debugging purposes, if any.
		/// </summary>
		public abstract string DebugName { get; }

		/// <summary>
		/// Gets a value indicating if this label has been marked.
		/// </summary>
		public abstract bool IsMarked { get; }
		#endregion
	}
}
