using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compiler.IR
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
		/// The static representation of this variable.
		/// </summary>
		public readonly MRepr StaticRepr;

		/// <summary>
		/// The constant value of this variable, if available.
		/// </summary>
		public readonly object ConstantValue;

		/// <summary>
		/// A value indicating if this variable is only initialized, and not modified afterwards.
		/// </summary>
		public readonly bool IsInitOnly;
		#endregion

		#region Constructors
		public Variable(string name, MRepr staticRepr, bool initOnly)
		{
			Contract.Requires(name != null);

			this.Name = name;
			this.StaticRepr = staticRepr;
			this.IsInitOnly = initOnly;
		}

		public Variable(string name, MRepr staticRepr, object constantValue, bool initOnly)
		{
			Contract.Requires(name != null);

			this.Name = name;
			this.StaticRepr = staticRepr;
			this.ConstantValue = constantValue;
			this.IsInitOnly = initOnly;
		}
		#endregion

		#region Properties
		public Type StaticCliType
		{
			get { return StaticRepr.CliType; }
		}
		#endregion

		#region Methods
		public override string ToString()
		{
			var str = new StringBuilder();

			str.Append(Name);
			str.Append(" : ");
			str.Append(StaticRepr);

			if (ConstantValue != null)
			{
				str.Append(" (");
				str.AppendFormat(CultureInfo.InvariantCulture, "{0}", ConstantValue);
				str.Append(')');
			}

			return str.ToString();
		}
		#endregion
	}
}
