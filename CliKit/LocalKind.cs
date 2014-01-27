using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CliKit
{
	/// <summary>
	/// Specifies the a kind of local as either a parameter or a local variable.
	/// </summary>
	[Serializable]
	public enum LocalKind
	{
		/// <summary>
		/// Denotes a formal method parameter.
		/// </summary>
		Parameter,

		/// <summary>
		/// Denotes a local variable used in the body of a method.
		/// </summary>
		Variable
	}
}
