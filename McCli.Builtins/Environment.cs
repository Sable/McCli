using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Builtins
{
	/// <summary>
	/// Implements MatLab builtins which relate to the execution environment.
	/// </summary>
	[MatlabLibrary]
	public static class Environment
	{
		// TODO: Find a way to reimplement ispc, isunix and ismac now that this is in a portable class library

		public static bool isstudent()
		{
			return false; // Muahaha !
		}

		#region Time
		public static double now()
		{
			// TODO: Compare this with actual MatLab output to see if it makes any sense.
			return (DateTime.Now - new DateTime(1, 1, 1)).TotalDays + 365;
		}

		public static MFullArray<double> clock()
		{
			var now = DateTime.Now;
			return MFullArray<double>.CreateRowVector(
				now.Year, now.Month, now.Day,
				now.Hour, now.Minute, now.Second + now.Millisecond / 1000.0);
		}

		public static MFullArray<double> etime(MArray<double> from, MArray<double> to)
		{
			Contract.Requires(from != null);
			Contract.Requires(to != null);

			var fromShape = from.Shape;
			var toShape = to.Shape;
			if (fromShape != toShape || fromShape.ColumnCount != 6 || fromShape.IsHigherDimensional)
				throw new MArrayShapeException();

			var result = new MFullArray<double>(fromShape.RowCount, 1);
			for (int rowIndex = 0; rowIndex < fromShape.RowCount; ++rowIndex)
				result[rowIndex] = (RowToDateTime(to, rowIndex) - RowToDateTime(from, rowIndex)).TotalSeconds;

			return result;
		}

		private static DateTime RowToDateTime(MArray<double> array, int rowIndex)
		{
			int rowCount = array.Shape.RowCount;
			double seconds = array[rowCount * 5 + rowIndex];
			return new DateTime(
				Utilities.ToInt(array[rowCount * 0 + rowIndex]),
				Utilities.ToInt(array[rowCount * 1 + rowIndex]),
				Utilities.ToInt(array[rowCount * 2 + rowIndex]),
				Utilities.ToInt(array[rowCount * 3 + rowIndex]),
				Utilities.ToInt(array[rowCount * 4 + rowIndex]),
				(int)seconds, (int)((seconds % 1) * 1000));
		}
		#endregion
	}
}
