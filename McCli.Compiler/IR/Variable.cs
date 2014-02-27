using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compiler.IR
{
	/// <summary>
	/// Represents a variable in a matlab program.
	/// </summary>
	[DebuggerDisplay("{Name} : {StaticRepr}")]
	public sealed class Variable
	{
		#region Fields
		/// <summary>
		/// The name of this variable.
		/// </summary>
		public readonly string Name;

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
		public Variable(string name)
		{
			Contract.Requires(name != null);

			this.Name = name;
		}

		public Variable(string name, MRepr staticRepr)
			: this(name)
		{
			this.StaticRepr = staticRepr;
		}

		public Variable(string name, object constantValue)
			: this(name)
		{
			Contract.Requires(constantValue is double || constantValue is char);

			this.StaticRepr = MClass.FromCliType(constantValue.GetType());
			this.ConstantValue = constantValue;
		}
		#endregion

		#region Properties
		public Type StaticCliType
		{
			get { return StaticRepr.CliType; }
		}
		#endregion
	}
}
