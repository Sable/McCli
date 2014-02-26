using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace McCli.Builtins
{
	/// <summary>
	/// Implements MatLab builtins for matrix analysis.
	/// http://www.mathworks.com/help/matlab/matrix-analysis.html
	/// </summary>
	[MatlabLibrary]
	public static class MatrixAnalysis
	{
		public static double norm(MArray<double> array)
		{
			Contract.Requires(array != null);

			if (array.IsVector)
			{
				double norm = 0;
				for (int i = 0; i < array.Count; ++i)
					norm += array[i] * array[i];
				return ExponentsAndLogarithms.sqrt(norm);
			}

			throw new NotImplementedException("Matrix 2-norm.");
		}
	}
}
