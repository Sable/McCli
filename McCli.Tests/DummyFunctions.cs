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

		public static void ConsumeDoubleScalar(double value) { }

		public static void SwapDoubleScalars(double in1, double in2, out double out1, out double out2)
		{
			out1 = in2;
			out2 = in1;
		}

		public static double InputTypeOverloadedIdentity(double value) { return value; }
		public static char InputTypeOverloadedIdentity(char value) { return value; }

		public static double StructuralClassOverloadedDoubleIdentity(double value) { return value; }
		public static MArray<double> StructuralClassOverloadedDoubleIdentity(MArray<double> value) { return value; }
	}
}
