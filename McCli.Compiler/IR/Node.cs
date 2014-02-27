using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compiler.IR
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

		/// <summary>
		/// Gets a short string representation of this node for debugging purposes.
		/// </summary>
		public abstract string ToDebugString();

		public override sealed string ToString()
		{
			return ToDebugString();
		}

		protected static string CommaSeparate(IEnumerable<Variable> variables)
		{
			var stringBuilder = new StringBuilder();
			using (var enumerator = variables.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					stringBuilder.Append(enumerator.Current.Name);
					while (enumerator.MoveNext())
					{
						stringBuilder.Append(", ");
						stringBuilder.Append(enumerator.Current.Name);
					}
				}
			}
			return stringBuilder.ToString();
		}
	}
}
