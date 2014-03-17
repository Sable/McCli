using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compiler.IR
{
	/// <summary>
	/// Represents an argument that can be an index and whose value can be
	/// either a variable or a colon (':') index.
	/// </summary>
	public struct IndexArgument
	{
		#region Fields
		public static readonly IndexArgument Colon = default(IndexArgument);

		private readonly Variable variable;
		#endregion

		#region Constructors
		public IndexArgument(Variable variable)
		{
			Contract.Requires(variable != null);
			this.variable = variable;
		}
		#endregion

		#region Properties
		public bool IsColon
		{
			get { return variable == null; }
		}

		public bool IsVariable
		{
			get { return variable != null; }
		}

		public Variable Variable
		{
			get { return variable; }
		}
		#endregion

		#region Methods
		public override string ToString()
		{
			return variable == null ? ":" : variable.Name;
		}
		#endregion

		#region Operators
		public static implicit operator IndexArgument(Variable variable)
		{
			return new IndexArgument(variable);
		}
		#endregion
	}
}
