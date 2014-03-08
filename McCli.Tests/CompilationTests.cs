using McCli.Compiler;
using McCli.Compiler.CodeGen;
using McCli.Compiler.IR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	[TestClass]
	public sealed class CompilationTests
	{
		private delegate void In2Out2<TIn1, TIn2, TOut1, TOut2>(
			TIn1 input1, TIn2 input2, out TOut1 output1, out TOut2 output2);
		private delegate void In0Out2<TOut1, TOut2>(out TOut1 output1, out TOut2 output2);

		private static readonly ImmutableArray.EmptyType none = ImmutableArray.Empty;

		private TDelegate CompileFunction<TDelegate>(
			ImmutableArray<Variable> inputs, ImmutableArray<Variable> outputs,
			params Statement[] statements) where TDelegate : class
		{
			var function = new Function("generated", inputs, outputs, ImmutableArray.Create(statements));
			var functionMethod = FunctionEmitter.Emit(function, MethodFactories.Dynamic, DummyFunctions.Lookup);
			return (TDelegate)(object)functionMethod.Method.CreateDelegate(typeof(TDelegate));
		}

		private static Variable Declare<T>(string name, bool initOnly = false)
		{
			var repr = MRepr.FromCliType(typeof(T));
			name += "_" + repr.ToString().Replace(" ", "");
			return new Variable(name, repr, initOnly);
		}

		[TestMethod]
		public void TestCopyAssignmentWithMArray()
		{
			// function result = copy(x)
			//   result = x;

			var input = Declare<MArray<double>>("input");
			var output = Declare<MArray<double>>("output");
			var function = CompileFunction<Func<MArray<double>, MArray<double>>>(
				input, output,
				new Copy(output, input));

			var argument = MFullArray<double>.CreateScalar(42);
			var result = function(argument);

			Assert.AreEqual(argument.Shape, result.Shape);
			Assert.AreEqual(argument[0], result[0]);
			Assert.AreNotSame(argument, result);
		}

		[TestMethod]
		public void TestCopyAssignmentWithScalar()
		{
			// function result = copy(x)
			//   result = x;

			var input = Declare<double>("input");
			var output = Declare<double>("output");
			var function = CompileFunction<Func<double, double>>(
				input, output,
				new Copy(output, input));

			Assert.AreEqual(42.0, function(42));
		}

		[TestMethod]
		public void TestParameterlessCall()
		{
			var output = Declare<double>("output");
			var function = CompileFunction<Func<double>>(
				none, output,
				new StaticCall(output, "Get42Scalar", ImmutableArray.Empty));

			Assert.AreEqual(42.0, function());
		}

		[TestMethod]
		public void TestDoubleLiteral()
		{
			var output = Declare<double>("output");
			var function = CompileFunction<Func<double>>(
				none, output,
				new Literal(output, 42.0));

			Assert.AreEqual(42.0, function());
		}

		[TestMethod]
		public void TestCharLiteral()
		{
			var output = Declare<char>("output");
			var function = CompileFunction<Func<char>>(
				none, output,
				new Literal(output, "a"));

			Assert.AreEqual('a', function());
		}

		[TestMethod]
		public void TestStringLiteral()
		{
			var output = Declare<MArray<char>>("output");
			var function = CompileFunction<Func<MArray<char>>>(
				none, output,
				new Literal(output, "foobar"));

			Assert.AreEqual("foobar", PseudoBuiltins.CharArrayToString(function()));
		}

		[TestMethod]
		public void TestParameterizedCall()
		{
			// function output = add(lhs, rhs)
			//   output = AddDoubleScalars(lhs, rhs)

			var lhsInput = Declare<double>("lhs");
			var rhsInput = Declare<double>("rhs");
			var output = Declare<double>("output");
			var function = CompileFunction<Func<double, double, double>>(
				new[] { lhsInput, rhsInput }, output,
				new StaticCall(output, "AddDoubleScalars", new[] { lhsInput, rhsInput }));

			Assert.AreEqual(708.0, function(42, 666));
		}

		[TestMethod]
		public void TestArrayLoad()
		{
			// function value = arrayload(array, index)
			//   value = array(index)

			var arrayInput = Declare<MArray<double>>("array");
			var indexInput = Declare<MArray<double>>("index");
			var valueOutput = Declare<MArray<double>>("value");
			var function = CompileFunction<Func<MArray<double>, MArray<double>, MArray<double>>>(
				new[] { arrayInput, indexInput }, valueOutput,
				new LoadParenthesized(valueOutput, arrayInput, indexInput));

			var arg1 = new MFullArray<double>(2, 1);
			arg1[0] = 42;
			arg1[1] = 666;

			Assert.AreEqual(arg1[0], function(arg1, 1.0).ToScalar());
			Assert.AreEqual(arg1[1], function(arg1, 2.0).ToScalar());
		}

		[TestMethod]
		public void TestArrayStore()
		{
			// function output = arraystore(array, index, value)
			//   array(index) = value;
			//   output = array;
			// end

			var arrayInput = Declare<MArray<double>>("array");
			var indexInput = Declare<MArray<double>>("index");
			var valueInput = Declare<MArray<double>>("value");
			var output = Declare<MArray<double>>("output");
			var function = CompileFunction<Func<MArray<double>, MArray<double>, MArray<double>, MArray<double>>>(
				new[] { arrayInput, indexInput, valueInput }, output,
				new StoreParenthesized(arrayInput, indexInput, valueInput),
				new Copy(output, arrayInput));

			var array = MFullArray<double>.CreateScalar(42);
			
			var result = function(array, 1, 666);
			Assert.IsTrue(result.IsScalar);
			Assert.AreEqual(42.0, array[0]);
			Assert.AreEqual(666.0, result[0]);
		}

		[TestMethod]
		public void TestIf()
		{
			// There is no such thing as a logical literal
			const double trueDouble = 42;
			const double falseDouble = 666;

			var input = Declare<MValue>("input");
			var output = Declare<double>("output");
			var function = CompileFunction<Func<MValue, double>>(
				input, output,
				new If(input,
					new Literal(output, trueDouble),
					new Literal(output, falseDouble)));

			Assert.AreEqual(trueDouble, function(MFullArray<double>.CreateScalar(1)));
			Assert.AreEqual(falseDouble, function(MFullArray<double>.CreateScalar(0)));
			Assert.AreEqual(falseDouble, function(MFullArray<double>.CreateRowVector(0, 0, 1, 0)));
			Assert.AreEqual(trueDouble, function(MFullArray<double>.CreateRowVector(1, 1, 1, 1)));
		}

		[TestMethod]
		public void TestWhile()
		{
			// function y = firstMultipleOver100(x)
			//   y = x
			//   while (y < 100) y *= xy;

			var input = Declare<double>("input");
			var oneHundredConstant = Declare<double>("100");
			var output = Declare<double>("output");
			var conditionVariable = Declare<bool>("condition");
			var function = CompileFunction<Func<double, double>>(
				input, output,
				new Copy(output, input),
				new Literal(oneHundredConstant, 100.0),
				new StaticCall(conditionVariable, "LessThanDoubleScalar", new[] { output, oneHundredConstant }),
				new While(conditionVariable,
					new StaticCall(output, "SquareDoubleScalar", output),
					new StaticCall(conditionVariable, "LessThanDoubleScalar", new[] { output, oneHundredConstant })
				));

			Assert.AreEqual(256.0, function(2));
			Assert.AreEqual(625.0, function(5));
			Assert.AreEqual(100.0, function(10));
		}

		[TestMethod]
		public void TestBreakWhile()
		{
			// while (42) break;
			// return 42;

			var output = Declare<double>("output");
			var function = CompileFunction<Func<double>>(
				none, output,
				new Literal(output, 42.0),
				new While(output,
					new Jump(JumpKind.Break),
					new Literal(output, 666.0))
				);

			Assert.AreEqual(42.0, function());
		}

		[TestMethod]
		public void TestBreakNestedWhile()
		{
			// x = 42;
			// while (x)
			//   while (x)
			//     break
			//     x = 13
			//   x = 666;
			//   break
			//   x = 7
			// return x

			var output = Declare<double>("output");
			var function = CompileFunction<Func<double>>(
				none, output,
				new Literal(output, 42.0),
				new While(output,
					new While(output,
						new Jump(JumpKind.Break),
						new Literal(output, 13)),
					new Literal(output, 666),
					new Jump(JumpKind.Break),
					new Literal(output, 7)
				));

			Assert.AreEqual(666.0, function());
		}

		[TestMethod]
		public void TestContinueInWhile()
		{
			// function output = test()
			//   output = 1
			//   while (output < 5)
			//     output = 42
			//     continue
			//     output = 666

			var output = Declare<double>("output");
			var constant = Declare<double>("constant");
			var condition = Declare<bool>("condition");
			var function = CompileFunction<Func<double>>(
				none, output,
				new Literal(output, 1.0),
				new Literal(constant, 5.0),
				new StaticCall(condition, "LessThanDoubleScalar", new[] { output, constant }),
				new While(condition,
					new Literal(output, 42.0),
					new StaticCall(condition, "LessThanDoubleScalar", new[] { output, constant }),
					new Jump(JumpKind.Continue),
					new Literal(output, 666.0)
					));

			Assert.AreEqual(42.0, function());
		}

		[TestMethod]
		public void TestForLoop()
		{
			// function result = repeatedsquare(n)
			//   result = 2;
			//   for i = 1:n
			//     result = SquareDoubleScalar(result)
			//   return result;

			var input = Declare<MArray<double>>("input");
			var output = Declare<MArray<double>>("output");
			var iteratorVariable = Declare<MArray<double>>("iterator");
			var oneConstant = Declare<MArray<double>>("one");

			var function = CompileFunction<Func<MArray<double>, MArray<double>>>(
				input, output,
				new Literal(output, 2.0),
				new Literal(oneConstant, 1.0),
				new RangeFor(iteratorVariable, oneConstant, null, input,
					new StaticCall(output, "SquareDoubleScalar", output)
				));

			Assert.AreEqual(2.0, function(0).ToScalar());
			Assert.AreEqual(4.0, function(1).ToScalar());
			Assert.AreEqual(16.0, function(2).ToScalar());
			Assert.AreEqual(256.0, function(3).ToScalar());
		}

		[TestMethod]
		public void TestReturn()
		{
			// function output = test()
			//   output = 42
			//   return
			//   output = 666

			var output = Declare<double>("output");
			var function = CompileFunction<Func<double>>(
				none, output,
				new Literal(output, 42.0),
				new Jump(JumpKind.Return),
				new Literal(output, 666.0));

			Assert.AreEqual(42.0, function());
		}

		[TestMethod]
		public void TestScalarToArrayPromotion()
		{
			// function array = foo(scalar)
			//   array = scalar

			var input = Declare<double>("input");
			var output = Declare<MArray<double>>("output");

			var function = CompileFunction<Func<double, MArray<double>>>(
				input, output, new Copy(output, input));

			Assert.AreEqual(42.0, function(42.0).ToScalar());
		}

		[TestMethod]
		public void TestMultipleOutputs()
		{
			// function [output1, output2] = swap(input1, input2)
			//   output1 = input2
			//   output2 = input1

			var input1 = Declare<double>("input1");
			var input2 = Declare<double>("input2");
			var output1 = Declare<double>("output1");
			var output2 = Declare<double>("output2");

			var function = CompileFunction<In2Out2<double, double, double, double>>(
				new[] { input1, input2 }, new[] { output1, output2 },
				new Copy(output1, input2),
				new Copy(output2, input1));

			double result1, result2;
			function(42, 666, out result1, out result2);
			Assert.AreEqual(666.0, result1);
			Assert.AreEqual(42.0, result2);
		}

		[TestMethod]
		public void TestInoutParameter()
		{
			// function x = square(x)
			//   x *= x

			var variable = Declare<double>("x");
			var function = CompileFunction<Func<double, double>>(
				variable, variable,
				new StaticCall(variable, "SquareDoubleScalar", variable));

			Assert.AreEqual(9.0, function(3));
		}

		[TestMethod]
		public void TestCallVoidReturningFunction()
		{
			// function x = square()
			//   x = 42
			//   ConsumeDouble(x)

			var variable = Declare<double>("x");
			var function = CompileFunction<Func<double>>(
				none, variable,
				new Literal(variable, 42),
				new StaticCall(none, "ConsumeDoubleScalar", variable));

			Assert.AreEqual(42.0, function());
		}

		[TestMethod]
		public void TestCallTwoOutputFunction()
		{
			// function [out1, out2] = swap(in1, in2)
			//   [out1, out2] = SwapDoubleScalars(in1, in2)

			var input1 = Declare<double>("input1");
			var input2 = Declare<double>("input2");
			var output1 = Declare<double>("output1");
			var output2 = Declare<double>("output2");
			var function = CompileFunction<In2Out2<double, double, double, double>>(
				new[] { input1, input2 }, new[] { output1, output2 },
				new StaticCall(new[] { output1, output2 }, "SwapDoubleScalars", new[] { input1, input2 }));

			double result1, result2;
			function(42, 666, out result1, out result2);
			Assert.AreEqual(666.0, result1);
			Assert.AreEqual(42.0, result2);
		}

		[TestMethod]
		public void TestOverloadingOnInputType()
		{
			var doubleInput = Declare<double>("doubleInput");
			var doubleOutput = Declare<double>("doubleOutput");
			var charInput = Declare<char>("charInput");
			var charOutput = Declare<char>("charOutput");

			var doubleFunction = CompileFunction<Func<double, double>>(
				doubleInput, doubleOutput, new StaticCall(doubleOutput, "InputTypeOverloadedIdentity", doubleInput));
			var charFunction = CompileFunction<Func<char, char>>(
				charInput, charOutput, new StaticCall(charOutput, "InputTypeOverloadedIdentity", charInput));

			Assert.AreEqual(42.0, doubleFunction(42));
			Assert.AreEqual('k', charFunction('k'));
		}

		[TestMethod]
		public void TestOverloadingOnStructuralClass()
		{
			var scalarInput = Declare<double>("ScalarInput");
			var scalarOutput = Declare<double>("ScalarOutput");
			var arrayInput = Declare<MArray<double>>("arrayInput");
			var arrayOutput = Declare<MArray<double>>("arrayOutput");

			var scalarFunction = CompileFunction<Func<double, double>>(
				scalarInput, scalarOutput, new StaticCall(scalarOutput, "StructuralClassOverloadedDoubleIdentity", scalarInput));
			var arrayFunction = CompileFunction<Func<MArray<double>, MArray<double>>>(
				arrayInput, arrayOutput, new StaticCall(arrayOutput, "StructuralClassOverloadedDoubleIdentity", arrayInput));

			Assert.AreEqual(42.0, scalarFunction(42));
			Assert.AreEqual(666.0, arrayFunction(666).ToScalar());
		}
	}
}
