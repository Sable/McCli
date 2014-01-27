using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace McCli
{
	[TestClass]
	public class TestArrays
	{
		[TestMethod]
		public void TestBasicIndexing()
		{
			var array = new MArray<double>(MArrayShape.Scalar);
		}

		public static void Test()
		{
			dynamic foo = (object)new int[2];
			object element = foo[3];
			Console.WriteLine(element);
		}
	}
}
