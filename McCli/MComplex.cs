using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// Represents a matlab complex value, with an imaginary and a real part.
	/// Complex values are handled transparently with real values by matlab programs.
	/// </summary>
	/// <typeparam name="TNumerical">The underlying primitive numerical type.</typeparam>
	public struct MComplex<TNumerical> where TNumerical : struct
	{
		#region Fields
		public readonly TNumerical RealPart, ImaginaryPart;
		#endregion

		#region Constructors
		public MComplex(TNumerical realPart)
		{
			this.RealPart = realPart;
			this.ImaginaryPart = default(TNumerical);
		}

		public MComplex(TNumerical realPart, TNumerical imaginaryPart)
		{
			this.RealPart = realPart;
			this.ImaginaryPart = imaginaryPart;
		}

		static MComplex()
		{
			// Ensure the primitive numerical type is one that supports complex numbers.
			var @class = MPrimitiveClass.FromCliType(typeof(TNumerical));
			Contract.Assert(@class != null && (@class.SupportedAttributes & MClassAttributes.Complex) != 0);
		}
		#endregion
	}
}
