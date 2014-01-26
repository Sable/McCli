using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// Represents a matlab complex value, with an imaginary and a real part.
	/// Complex values are handled transparently with real values by matlab programs.
	/// </summary>
	/// <typeparam name="T">The underlying primitive numerical type.</typeparam>
	public struct MComplex<T> where T : struct
	{
		#region Fields
		public readonly T RealPart, ImaginaryPart;
		#endregion

		#region Constructors
		public MComplex(T realPart)
		{
			this.RealPart = realPart;
			this.ImaginaryPart = default(T);
		}

		public MComplex(T realPart, T imaginaryPart)
		{
			this.RealPart = realPart;
			this.ImaginaryPart = imaginaryPart;
		}
		#endregion
	}
}
