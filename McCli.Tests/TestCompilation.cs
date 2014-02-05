using McCli.Compilation;
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
		private static readonly Variable doubleArrayInput = new Variable("input", VariableKind.Input, MPrimitiveClass.Double.ArrayRepr);
		private static readonly Variable doubleArrayInput2 = new Variable("input2", VariableKind.Input, MPrimitiveClass.Double.ArrayRepr);
		private static readonly Variable doubleArrayInput3 = new Variable("input3", VariableKind.Input, MPrimitiveClass.Double.ArrayRepr);
		private static readonly Variable doubleArrayOutput = new Variable("output", VariableKind.Output, MPrimitiveClass.Double.ArrayRepr);
		private FunctionLookup functionLookup;

		[TestInitialize]
		public void Initialize()
		{
			var functionTable = new FunctionTable();
			functionTable.AddStatic(typeof(Builtins));
			functionLookup = functionTable.Lookup;
		}

		private TDelegate CompileFunction<TDelegate>(Variable[] inputs, Variable output,
			params Statement[] statements) where TDelegate : class
		{
			var immutableInputs = inputs == null ? ImmutableArray<Variable>.Empty : ImmutableArray.Create(inputs);
			var immutableOutput = output == null ? ImmutableArray<Variable>.Empty : ImmutableArray.Create(output);
			var function = new Function("generated", immutableInputs, immutableOutput, ImmutableArray.Create(statements));
			var method = FunctionBodyEmitter.Emit(function, MethodFactories.Dynamic, functionLookup);
			return (TDelegate)(object)method.CreateDelegate(typeof(TDelegate));
		}

		[TestMethod]
		public void TestCopyIRNode()
		{
			var function = CompileFunction<Func<MArray<double>, MArray<double>>>(
				new[] { doubleArrayInput }, doubleArrayOutput,
				new Copy(doubleArrayInput, doubleArrayOutput));

			var argument = MDenseArray<double>.CreateScalar(42);
			var result = function(argument);

			Assert.AreEqual(argument.Shape, result.Shape);
			Assert.AreEqual(argument[0], result[0]);
		}

		[TestMethod]
		public void TestParameterlessCall()
		{
			var function = CompileFunction<Func<MArray<double>>>(
				null, doubleArrayOutput,
				new StaticCall("eye", ImmutableArray.Empty, doubleArrayOutput));

			var result = function();
			Assert.AreEqual(result.ToScalar(), 1.0);
		}

		[TestMethod]
		public void TestLiteral()
		{
			var function = CompileFunction<Func<MArray<double>>>(
				null, doubleArrayOutput,
				new Literal(doubleArrayOutput, 42.0));

			var result = function();
			Assert.AreEqual(result.ToScalar(), 42.0);
		}

		[TestMethod]
		public void TestParameterizedCall()
		{
			var function = CompileFunction<Func<MArray<double>, MArray<double>, MArray<double>>>(
				new[] { doubleArrayInput, doubleArrayInput2 }, doubleArrayOutput,
				new StaticCall("plus", new[] { doubleArrayInput, doubleArrayInput2 }, doubleArrayOutput));

			var arg1 = new MDenseArray<double>(2, 1);
			arg1[0] = 42;
			arg1[1] = 666;
			var result = function(arg1, MDenseArray<double>.CreateScalar(1));
			Assert.AreEqual(arg1.Shape, result.Shape);
			Assert.AreEqual(43, result[0]);
			Assert.AreEqual(667, result[1]);
		}

		[TestMethod]
		public void TestArrayLoad()
		{
			var function = CompileFunction<Func<MArray<double>, MArray<double>, MArray<double>>>(
				new[] { doubleArrayInput, doubleArrayInput2 }, doubleArrayOutput,
				new LoadCall(doubleArrayInput, doubleArrayInput2, doubleArrayOutput));

			var arg1 = new MDenseArray<double>(2, 1);
			arg1[0] = 42;
			arg1[1] = 666;

			Assert.AreEqual(arg1[0], function(arg1, 1.0).ToScalar());
			Assert.AreEqual(arg1[1], function(arg1, 2.0).ToScalar());
		}

		[TestMethod]
		public void TestArrayStore()
		{
			var function = CompileFunction<Func<MArray<double>, MArray<double>, MArray<double>, MArray<double>>>(
				new[] { doubleArrayInput, doubleArrayInput2, doubleArrayInput3 }, doubleArrayOutput,
				new StoreIndexed(doubleArrayInput, doubleArrayInput2, doubleArrayInput3),
				new Copy(doubleArrayOutput, doubleArrayInput));

			var array = MDenseArray<double>.CreateScalar(42);
			
			var result = function(array, 1, 666);
			Assert.IsTrue(result.IsScalar);
			Assert.AreEqual(42.0, array[0]);
			Assert.AreEqual(666.0, result[0]);
		}
	}
}
