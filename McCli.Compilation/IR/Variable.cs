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
		public readonly MRepr StaticType;

		/// <summary>
		/// The constant value of this variable, if available.
		/// </summary>
		public readonly MValue ConstantValue;
		#endregion

		#region Constructors
		public Variable(string name, VariableKind kind)
		{
			Contract.Requires(name != null);

			this.Name = name;
			this.Kind = kind;
		}

		public Variable(string name, VariableKind kind, MRepr staticType)
			: this(name, kind)
		{
			this.StaticType = staticType;
		}

		public Variable(string name, VariableKind kind, MValue constantValue)
			: this(name, kind)
		{
			Contract.Requires(constantValue != null);

			this.StaticType = constantValue.Type;
			this.ConstantValue = constantValue;
		}
		#endregion
	}
}
