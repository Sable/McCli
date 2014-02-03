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
		public readonly MClass StaticClass;

		/// <summary>
		/// The constant value of this variable, if available.
		/// </summary>
		public readonly MValue ConstantValue;
		#endregion

		#region Constructors
		public Variable(string name, VariableKind kind, MClass staticClass)
		{
			Contract.Requires(name != null);

			this.Name = name;
			this.Kind = kind;
			this.StaticClass = staticClass;
		}

		public Variable(string name, VariableKind kind, MValue constantValue)
		{
			Contract.Requires(name != null);
			Contract.Requires(constantValue != null);

			this.Name = name;
			this.Kind = kind;
			this.ConstantValue = constantValue;
			this.StaticClass = constantValue.Class;
		}
		#endregion
	}
}
