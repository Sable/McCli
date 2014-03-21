using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McCli.Builtins
{
	/// <summary>
	/// Implements MatLab builtins for random number generation.
	/// http://www.mathworks.com/help/matlab/random-number-generation.html
	/// </summary>
	[MatlabLibrary]
	public static class RandomNumberGeneration
	{
		#region Fields
		internal static readonly Random threadSharedRandom = new Random();
		#endregion

		#region Methods
		#region rand
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
			var result = ArrayCreation.zeros(rowCount, columnCount);
			lock (threadSharedRandom)
			{
				for (int i = 0; i < result.Count; ++i)
					result[i] = threadSharedRandom.NextDouble();
			}
			
			return result;
		}
		#endregion

		#region randn
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
			var result = ArrayCreation.zeros(rowCount, columnCount);
			var resultArray = result.BackingArray;
			int count = result.Count;
			lock (threadSharedRandom)
			{
				// The Box-Muller algorithm generates two values at a time,
				// so unroll the loop to take advantage of this.
				int unrolledCount = count / 2 * 2;
				for (int i = 0; i < unrolledCount; i += 2)
				{
					double z1, z2;
					RandomNormalBoxMuller_NoLock(out z1, out z2);
					resultArray[i + 0] = z1;
					resultArray[i + 1] = z2;
				}

				// For odd-sized arrays, generate the last value.
				if ((count % 2) == 1)
				{
					double z1, z2;
					RandomNormalBoxMuller_NoLock(out z1, out z2);
					resultArray[count - 1] = z1;
				}
			}

			return result;
		}

		internal static void RandomNormalBoxMuller_NoLock(out double z1, out double z2)
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

		#region randi
		public static double randi(double imax)
		{
			var exclusiveMax = PseudoBuiltins.ToInt(imax + 1);
			lock (threadSharedRandom)
				return threadSharedRandom.Next(1, exclusiveMax);
		}

		public static MFullArray<double> randi(double imax, double n)
		{
			return randi(imax, n, n);
		}

		public static MFullArray<double> randi(double imax, double rowCount, double columnCount)
		{
			var exclusiveMax = PseudoBuiltins.ToInt(imax + 1);
			var result = MFullArray<double>.CreateWithShape(rowCount, columnCount);
			int count = result.Count;
			lock (threadSharedRandom)
			{
				for (int i = 0; i < count; ++i)
					result.BackingArray[i] = threadSharedRandom.Next(0, exclusiveMax);
			}
			return result;
		}
		#endregion
		#endregion
	}
}
