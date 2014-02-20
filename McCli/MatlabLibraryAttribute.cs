using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McCli
{
	/// <summary>
	/// Marks a class as providing Matlab-callable functions.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class MatlabLibraryAttribute : Attribute { }
}
