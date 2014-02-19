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
		/// The static representation of this variable, if available.
		/// </summary>
		public readonly MRepr StaticRepr;

		/// <summary>
		/// The constant value of this variable, if available.
		/// </summary>
		public readonly object ConstantValue;
		#endregion

		#region Constructors
		public Variable(string name, VariableKind kind)
		{
			Contract.Requires(name != null);

			this.Name = name;
			this.Kind = kind;
		}

		public Variable(string name, VariableKind kind, MRepr staticRepr)
			: this(name, kind)
		{
			this.StaticRepr = staticRepr;
		}

		public Variable(string name, VariableKind kind, object constantValue)
			: this(name, kind)
		{
			Contract.Requires(constantValue is double || constantValue is char);

			this.StaticRepr = MClass.FromCliType(constantValue.GetType());
			this.ConstantValue = constantValue;
		}
		#endregion
	}
}
