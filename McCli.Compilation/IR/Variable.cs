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
		#region Fields
		/// <summary>
		/// The name of this variable.
		/// </summary>
		public readonly string Name;

		/// <summary>
		/// The kind of variable, describing how it was declared.
		/// </summary>
		public readonly VariableKind Kind;

		/// <summary>
		/// The static type of this variable, if available.
		/// </summary>
		public readonly MType StaticType;

		/// <summary>
		/// A value indicating whether this variable is always scalar.
		/// </summary>
		public readonly bool Scalar;
		#endregion

		#region Constructors
		public Variable(string name, VariableKind kind, MType staticType, bool scalar)
		{
			Contract.Requires(name != null);

			this.Name = name;
			this.Kind = kind;
			this.StaticType = staticType;
			this.Scalar = scalar;
		}

		public Variable(string name, VariableKind kind, MType staticType)
			: this(name, kind, staticType, scalar: false) { }

		public Variable(string name, VariableKind kind)
			: this(name, kind, staticType: null, scalar: false) { }
		#endregion
	}
}
