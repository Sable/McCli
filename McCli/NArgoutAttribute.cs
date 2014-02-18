using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// Marks the last parameter of a method implementing a MatLab function
	/// as representing the number of outputs it is expected to return.
	/// </summary>
	/// <example>
	/// <c>a = foo()</c> would have a nargout of 1.
	/// <c>[a, b] = foo()</c> would have a nargout of 2.
	/// </example>
	[AttributeUsage(AttributeTargets.Parameter)]
	public sealed class NArgoutAttribute : Attribute
	{
		public NArgoutAttribute() { }
	}
}
