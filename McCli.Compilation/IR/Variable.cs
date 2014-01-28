using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compilation.IR
{
	/// <summary>
	/// Represents a variable in a matlab program.
	/// </summary>
	public sealed class Variable
	{
		public readonly string Name;
		public readonly VariableKind Kind;
		public readonly MType StaticType;

		public Variable(string name, VariableKind kind, MType staticType)
		{
			Contract.Requires(name != null);

			this.Name = name;
			this.Kind = kind;
			this.StaticType = staticType;
		}
	}
}
