using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// Specifies the layers around a MatLab class that define the full type.
	/// </summary>
	[Flags]
	public enum MTypeLayers
	{
		/// <summary>
		/// Indicates that the type is the MatLab class in its scalar form.
		/// </summary>
		None = 0,

		/// <summary>
		/// Indicates an <see cref="MComplex{T}"/> layer.
		/// </summary>
		Complex = 1 << 0,

		/// <summary>
		/// Indicates an <see cref="MArray{T}"/> layer (either of dense or sparse).
		/// </summary>
		Array = 1 << 1,

		DenseArrayFlag = 1 << 2,
		SparseMatrixFlag = 1 << 3,

		/// <summary>
		/// Indicates an <see cref="MDenseArray{T}"/> layer, which implies <see cref="MArray{T}"/>.
		/// </summary>
		DenseArray = Array | DenseArrayFlag,

		SparseMatrix = Array | SparseMatrixFlag,
	}
}
