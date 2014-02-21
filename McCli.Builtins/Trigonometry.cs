using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace McCli.Builtins
{
	/// <summary>
	/// Implements MatLab trigonometric builtins
	/// </summary>
	[MatlabLibrary]
	public static class Trigonometry
	{
		public static double pi()
		{
			return Math.PI;
		}

		public static MArray<double> cos(MArray<double> array)
		{
			return Arrays.arrayfun(cos, array);
		}

		private static double cos(double value)
		{
			return Math.Cos(value);
		}

		public static MArray<double> sin(MArray<double> array)
		{
			return Arrays.arrayfun(sin, array);
		}

		private static double sin(double value)
		{
			return Math.Sin(value);
		}

		public static MArray<double> tan(MArray<double> array)
		{
			return Arrays.arrayfun(tan, array);
		}

		private static double tan(double value)
		{
			return Math.Tan(value);
		}

		// TODO: All other trig' functions
	}
}
