using McCli.Compilation.CodeGen;
using McCli.Compilation.IR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	[TestClass]
	public sealed class TestCompilation
	{
		private delegate void DoubleInDoubleOutFunc(MArray<double> @in, ref MArray<double> @out);

		[TestMethod]
		public void TestDoubleArrayIdentity()
		{
			var input = new Variable("input", VariableKind.Input, MPrimitiveClass.Double);
			var output = new Variable("output", VariableKind.Output, MPrimitiveClass.Double);
			
			var function = new Function("identity",
				ImmutableArray.Create(input),
				ImmutableArray.Create(output),
				ImmutableArray.Create<Statement>(
					new Copy(input, output))
				);

			var method = FunctionBodyEmitter.Emit(function, MethodFactories.Dynamic);
			var @delegate = (DoubleInDoubleOutFunc)method.CreateDelegate(typeof(DoubleInDoubleOutFunc));

			var argument = MArray<double>.CreateScalar(42);
			MArray<double> result = null;
			@delegate(argument, ref result);

			Assert.AreEqual(argument.Shape, result.Shape);
			Assert.AreEqual(argument[0], result[0]);
		}
	}
}
