﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McCli.Builtins
{
	/// <summary>
	/// Implements random number generation-related MatLab builtins.
	/// </summary>
	[MatlabLibrary]
	public static class Randomness
	{
		#region Fields
		private static readonly Random threadSharedRandom = new Random();
		#endregion

		#region Methods
		public static double rand()
		{
			lock (threadSharedRandom) return threadSharedRandom.NextDouble();
		}

		public static MFullArray<double> rand(double n)
		{
			return rand(n, n);
		}

		public static MFullArray<double> rand(double rowCount, double columnCount)
		{
			var shape = Utilities.ToShape(rowCount, columnCount);
			var result = new MFullArray<double>(shape);
			lock (threadSharedRandom)
			{
				for (int i = 0; i < shape.Count; ++i)
					result[i] = threadSharedRandom.NextDouble();
			}
			
			return result;
		}

		public static double randn()
		{
			double result, dummy;
			lock (threadSharedRandom) RandomNormalBoxMuller_NoLock(out result, out dummy);
			return result;
		}

		public static MFullArray<double> randn(double n)
		{
			return randn(n, n);
		}

		public static MFullArray<double> randn(double rowCount, double columnCount)
		{
			int rowCountInt = Utilities.ToInt(rowCount);
			int columnCountInt = Utilities.ToInt(columnCount);

			var shape = Utilities.ToShape(rowCount, columnCount);
			var result = new MFullArray<double>(shape);
			int count = shape.Count;
			lock (threadSharedRandom)
			{
				// The Box-Muller algorithm generates two values at a time,
				// so unroll the loop to take advantage of this.
				for (int i = 0; i < count; i += 2)
				{
					double z1, z2;
					RandomNormalBoxMuller_NoLock(out z1, out z2);
					result[i + 0] = z1;
					result[i + 1] = z2;
				}

				// For odd-sized arrays, generate the last value.
				if ((count & 1) == 1)
				{
					double z1, z2;
					RandomNormalBoxMuller_NoLock(out z1, out z2);
					result[count - 1] = z1;
				}
			}

			return result;
		}

		private static void RandomNormalBoxMuller_NoLock(out double z1, out double z2)
		{
			// Source: http://en.wikipedia.org/wiki/Box%E2%80%93Muller_transform
			var u1 = threadSharedRandom.NextDouble();
			var u2 = threadSharedRandom.NextDouble();

			var r = Math.Sqrt(-2 * Math.Log(u1));
			var t = 2 * Math.PI * u2;

			z1 = r * Math.Cos(t);
			z2 = r * Math.Sin(t);
		}
		#endregion
	}
}