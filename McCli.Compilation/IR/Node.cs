using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compilation.IR
{
	/// <summary>
	/// Abstract base class of the IR nodes.
	/// </summary>
	public abstract class Node
	{
		/// <summary>
		/// Implements the visitor pattern and dynamic dispatch
		/// by invoking the correct visit method on a visitor
		/// based on the runtime type of this node.
		/// </summary>
		/// <param name="visitor">The visitor.</param>
		public abstract void Accept(Visitor visitor);
	}
}
