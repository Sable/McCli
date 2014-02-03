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
	/// <typeparam name="TNumeric">The underlying primitive numerical type.</typeparam>
	public struct MComplex<TNumeric> where TNumeric : struct
	{
		#region Fields
		public readonly TNumeric RealPart, ImaginaryPart;
		#endregion

		#region Constructors
		public MComplex(TNumeric realPart)
		{
			this.RealPart = realPart;
			this.ImaginaryPart = default(TNumeric);
		}

		public MComplex(TNumeric realPart, TNumeric imaginaryPart)
		{
			this.RealPart = realPart;
			this.ImaginaryPart = imaginaryPart;
		}

		static MComplex()
		{
			// Ensure the primitive numerical type is one that supports complex numbers.
			var @class = MPrimitiveClass.FromCliType(typeof(TNumeric));
			Contract.Assert(@class != null && (@class.SupportedAttributes & MClassAttributes.Complex) != 0);
		}
		#endregion
	}
}
