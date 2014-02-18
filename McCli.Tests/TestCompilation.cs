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
		private static readonly Variable anyInput = new Variable("input", VariableKind.Input, MRepr.Any);
		private static readonly Variable doubleArrayInput = new Variable("input", VariableKind.Input, MPrimitiveClass.Double.ArrayRepr);
		private static readonly Variable doubleArrayInput2 = new Variable("input2", VariableKind.Input, MPrimitiveClass.Double.ArrayRepr);
		private static readonly Variable doubleArrayInput3 = new Variable("input3", VariableKind.Input, MPrimitiveClass.Double.ArrayRepr);
		private static readonly Variable doubleArrayOutput = new Variable("output", VariableKind.Output, MPrimitiveClass.Double.ArrayRepr);
		private static readonly Variable doubleArrayLocal1 = new Variable("double1", VariableKind.Local, MPrimitiveClass.Double.ArrayRepr);
		private static readonly Variable doubleArrayLocal2 = new Variable("double2", VariableKind.Local, MPrimitiveClass.Double.ArrayRepr);
		private static readonly Variable logicalArrayLocal = new Variable("logicals", VariableKind.Local, MPrimitiveClass.Logical.ArrayRepr);
		private static readonly Variable logicalArrayOutput = new Variable("output", VariableKind.Output, MPrimitiveClass.Logical.ArrayRepr);
		private static readonly Variable charArrayOutput = new Variable("output", VariableKind.Output, MPrimitiveClass.Char.ArrayRepr);
		private FunctionLookup functionLookup;

		[TestInitialize]
		public void Initialize()
		{
			var functionTable = new FunctionTable();
			functionTable.AddStatic(typeof(McCli.Builtins.Operators));
			functionTable.AddStatic(typeof(McCli.Builtins.Classes));
			functionTable.AddStatic(typeof(McCli.Builtins.Arrays));
			functionTable.AddStatic(typeof(McCli.Builtins.Environment));
			functionTable.AddStatic(typeof(McCli.Builtins.Floats));
			functionTable.AddStatic(typeof(McCli.Builtins.Complex));
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
		public void TestCopyAssignment()
		{
			// result = copy(x): result = clone(x)

			var function = CompileFunction<Func<MArray<double>, MArray<double>>>(
				new[] { doubleArrayInput }, doubleArrayOutput,
				new Copy(doubleArrayInput, doubleArrayOutput));

			var argument = MFullArray<double>.CreateScalar(42);
			var result = function(argument);

			Assert.AreEqual(argument.Shape, result.Shape);
			Assert.AreEqual(argument[0], result[0]);
			Assert.AreNotSame(argument, result);
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
		public void TestDoubleLiteral()
		{
			var function = CompileFunction<Func<MArray<double>>>(
				null, doubleArrayOutput,
				new Literal(doubleArrayOutput, 42.0));

			var result = function();
			Assert.AreEqual(result.ToScalar(), 42.0);
		}

		[TestMethod]
		public void TestCharLiteral()
		{
			var function = CompileFunction<Func<MArray<char>>>(
				null, charArrayOutput,
				new Literal(charArrayOutput, 'a'));

			Assert.AreEqual('a', function().ToScalar());
		}

		[TestMethod]
		public void TestParameterizedCall()
		{
			var function = CompileFunction<Func<MArray<double>, MArray<double>, MArray<double>>>(
				new[] { doubleArrayInput, doubleArrayInput2 }, doubleArrayOutput,
				new StaticCall("plus", new[] { doubleArrayInput, doubleArrayInput2 }, doubleArrayOutput));

			var arg1 = new MFullArray<double>(2, 1);
			arg1[0] = 42;
			arg1[1] = 666;
			var result = function(arg1, MFullArray<double>.CreateScalar(1));
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

			var arg1 = new MFullArray<double>(2, 1);
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

			var array = MFullArray<double>.CreateScalar(42);
			
			var result = function(array, 1, 666);
			Assert.IsTrue(result.IsScalar);
			Assert.AreEqual(42.0, array[0]);
			Assert.AreEqual(666.0, result[0]);
		}

		[TestMethod]
		public void TestIf()
		{
			const double trueDouble = 42;
			const double falseDouble = 666;

			var function = CompileFunction<Func<MValue, MArray<double>>>(
				new[] { anyInput }, doubleArrayOutput,
				new If(anyInput,
					new Literal(doubleArrayOutput, trueDouble),
					new Literal(doubleArrayOutput, falseDouble)));

			Assert.AreEqual(trueDouble, function(MFullArray<double>.CreateScalar(1)).ToScalar());
			Assert.AreEqual(falseDouble, function(MFullArray<double>.CreateScalar(0)).ToScalar());
			Assert.AreEqual(falseDouble, function(MFullArray<double>.CreateRowVector(0, 0, 1, 0)).ToScalar());
			Assert.AreEqual(trueDouble, function(MFullArray<double>.CreateRowVector(1, 1, 1, 1)).ToScalar());
		}

		[TestMethod]
		public void TestWhile()
		{
			// y = x;
			// while (y < 100) y *= x;

			var function = CompileFunction<Func<MArray<double>, MArray<double>>>(
				new[] { doubleArrayInput }, doubleArrayOutput,
				new Copy(doubleArrayInput, doubleArrayOutput),
				new Literal(doubleArrayLocal1, 100.0),
				new StaticCall("lt", new[] { doubleArrayOutput, doubleArrayLocal1 }, logicalArrayLocal),
				new While(logicalArrayLocal, 
					new StaticCall("times", new[] { doubleArrayOutput, doubleArrayInput }, doubleArrayOutput),
					new StaticCall("lt", new[] { doubleArrayOutput, doubleArrayLocal1 }, logicalArrayLocal)
				));

			Assert.AreEqual(125.0, function(MFullArray<double>.CreateScalar(5)).ToScalar());
			Assert.AreEqual(100.0, function(MFullArray<double>.CreateScalar(10)).ToScalar());
			Assert.AreEqual(101.0, function(MFullArray<double>.CreateScalar(101)).ToScalar());
		}

		[TestMethod]
		public void TestBreak()
		{
			// while (42) break;
			// return 42;

			var function = CompileFunction<Func<MArray<double>>>(
				null, doubleArrayOutput,
				new Literal(doubleArrayOutput, 42.0),
				new While(doubleArrayOutput, new Jump(JumpKind.Break)));

			Assert.AreEqual(42.0, function().ToScalar());
		}

		[TestMethod]
		public void TestBreaksInNestedWhiles()
		{
			// x = 42;
			// while (x)
			// {
			//   while (x) break;
			//   x = 666;
			//   break;
			// }
			// return x;

			var function = CompileFunction<Func<MArray<double>>>(
				null, doubleArrayOutput,
				new Literal(doubleArrayOutput, 42.0),
				new While(doubleArrayOutput, 
					new While(doubleArrayOutput, new Jump(JumpKind.Break)),
					new Literal(doubleArrayOutput, 666.0),
					new Jump(JumpKind.Break)
				));

			Assert.AreEqual(666.0, function().ToScalar());
		}

		[TestMethod]
		public void TestForLoop()
		{
			// total = sum(values)
			// total = 0;
			// for value = values
			// {
			//   total = total + value
			// }
			// return total;

			var valuesInput = doubleArrayInput;
			var totalOutput = doubleArrayOutput;
			var valueVariable = doubleArrayLocal1;

			var function = CompileFunction<Func<MArray<double>, MArray<double>>>(
				new[] { valuesInput }, totalOutput,
				new Literal(totalOutput, 0.0),
				new For(valueVariable, valuesInput,
					new StaticCall("plus", new[] { totalOutput, valueVariable }, totalOutput)
				));

			Assert.AreEqual(0.0, function(MFullArray<double>.CreateEmpty()).ToScalar());
			Assert.AreEqual(42.0, function(MFullArray<double>.CreateScalar(42.0)).ToScalar());
			Assert.AreEqual(2.0, function(MFullArray<double>.CreateRowVector(1.0, 1.0)).ToScalar());
		}
	}
}
