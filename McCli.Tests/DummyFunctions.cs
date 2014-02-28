using McCli.Compiler;
using McCli.Compiler.CodeGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// A set of functions for unit tests that do function calls.
	/// </summary>
	[MatlabLibrary]
	public static class DummyFunctions
	{
		private static readonly FunctionLookup lookup;

		static DummyFunctions()
		{
			var functionTable = new FunctionTable();
			functionTable.AddMethodsFromType(typeof(DummyFunctions));
			lookup = functionTable.Lookup;
		}

		public static FunctionLookup Lookup
		{
			get { return lookup; }
		}

		public static double Get42Scalar()
		{
			return 42;
		}

		public static double AddDoubleScalars(double lhs, double rhs)
		{
			return lhs + rhs;
		}

		public static double SquareDoubleScalar(double value)
		{
			return value * value;
		}

		public static bool LessThanDoubleScalar(double lhs, double rhs)
		{
			return lhs < rhs;
		}
	}
}
