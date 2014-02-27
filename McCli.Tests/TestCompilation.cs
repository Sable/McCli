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
	public sealed class TestCompilation
	{
		private delegate void In2Out2<TIn1, TIn2, TOut1, TOut2>(
			TIn1 input1, TIn2 input2, out TOut1 output1, out TOut2 output2);
		private delegate void In0Out2<TOut1, TOut2>(out TOut1 output1, out TOut2 output2);

		private static readonly ImmutableArray.EmptyType none = ImmutableArray.Empty;

		private FunctionLookup functionLookup;

		[TestInitialize]
		public void Initialize()
		{
			var functionTable = new FunctionTable();
			functionTable.AddMethodsFromAssembly(typeof(Builtins.ArrayCreation).Assembly);
			functionLookup = functionTable.Lookup;
		}

		private TDelegate CompileFunction<TDelegate>(
			ImmutableArray<Variable> inputs, ImmutableArray<Variable> outputs,
			params Statement[] statements) where TDelegate : class
		{
			var function = new Function("generated", inputs, outputs, ImmutableArray.Create(statements));
			var functionMethod = FunctionEmitter.Emit(function, MethodFactories.Dynamic, functionLookup);
			return (TDelegate)(object)functionMethod.Method.CreateDelegate(typeof(TDelegate));
		}

		private static Variable Declare<T>(string name)
		{
			var repr = MRepr.FromCliType(typeof(T));
			name += "_" + repr.ToString().Replace(" ", "");
			return new Variable(name, repr);
		}

		[TestMethod]
		public void TestCopyAssignment()
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
		public void TestParameterlessCall()
		{
			var output = Declare<MArray<double>>("output");
			var function = CompileFunction<Func<MArray<double>>>(
				none, output,
				new StaticCall(output, "eye", ImmutableArray.Empty));

			Assert.AreEqual(1.0, function().ToScalar());
		}

		[TestMethod]
		public void TestDoubleLiteral()
		{
			var output = Declare<MArray<double>>("output");
			var function = CompileFunction<Func<MArray<double>>>(
				none, output,
				new Literal(output, 42.0));

			Assert.AreEqual(42.0, function().ToScalar());
		}

		[TestMethod]
		public void TestCharLiteral()
		{
			var output = Declare<MArray<char>>("output");
			var function = CompileFunction<Func<MArray<char>>>(
				none, output,
				new Literal(output, 'a'));

			Assert.AreEqual('a', function().ToScalar());
		}

		[TestMethod]
		public void TestParameterizedCall()
		{
			var lhsInput = Declare<MArray<double>>("lhs");
			var rhsInput = Declare<MArray<double>>("rhs");
			var output = Declare<MArray<double>>("output");
			var function = CompileFunction<Func<MArray<double>, MArray<double>, MArray<double>>>(
				new[] { lhsInput, rhsInput }, output,
				new StaticCall(output, "plus", new[] { lhsInput, rhsInput }));

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
			// function output = subsasgn(array, index, value)
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
			var output = Declare<MArray<double>>("output");
			var function = CompileFunction<Func<MValue, MArray<double>>>(
				input, output,
				new If(input,
					new Literal(output, trueDouble),
					new Literal(output, falseDouble)));

			Assert.AreEqual(trueDouble, function(MFullArray<double>.CreateScalar(1)).ToScalar());
			Assert.AreEqual(falseDouble, function(MFullArray<double>.CreateScalar(0)).ToScalar());
			Assert.AreEqual(falseDouble, function(MFullArray<double>.CreateRowVector(0, 0, 1, 0)).ToScalar());
			Assert.AreEqual(trueDouble, function(MFullArray<double>.CreateRowVector(1, 1, 1, 1)).ToScalar());
		}

		[TestMethod]
		public void TestWhile()
		{
			// function y = squareuntil100(x)
			//   y = x;
			//   while (y < 100) y *= x;

			var input = Declare<MArray<double>>("input");
			var oneHundredConstant = Declare<MArray<double>>("100");
			var output = Declare<MArray<double>>("output");
			var conditionVariable = Declare<MArray<bool>>("condition");
			var function = CompileFunction<Func<MArray<double>, MArray<double>>>(
				input, output,
				new Copy(output, input),
				new Literal(oneHundredConstant, 100.0),
				new StaticCall(conditionVariable, "lt", new[] { output, oneHundredConstant }),
				new While(conditionVariable, 
					new StaticCall(output, "times", new[] { output, input }),
					new StaticCall(conditionVariable, "lt", new[] { output, oneHundredConstant })
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

			var output = Declare<MArray<double>>("output");
			var function = CompileFunction<Func<MArray<double>>>(
				none, output,
				new Literal(output, 42.0),
				new While(output, new Jump(JumpKind.Break)));

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

			var output = Declare<MArray<double>>("output");
			var function = CompileFunction<Func<MArray<double>>>(
				none, output,
				new Literal(output, 42.0),
				new While(output,
					new While(output, new Jump(JumpKind.Break)),
					new Literal(output, 666.0),
					new Jump(JumpKind.Break)
				));

			Assert.AreEqual(666.0, function().ToScalar());
		}

		[TestMethod]
		public void TestForLoop()
		{
			// function result = factorial(n)
			//   result = 1;
			//   for i = 1:n
			//   {
			//     result = result * i;
			//   }
			//   return result;

			var input = Declare<MArray<double>>("input");
			var output = Declare<MArray<double>>("output");
			var iteratorVariable = Declare<MArray<double>>("iterator");
			var oneConstant = Declare<MArray<double>>("one");

			var function = CompileFunction<Func<MArray<double>, MArray<double>>>(
				input, output,
				new Literal(output, 1.0),
				new Literal(oneConstant, 1.0),
				new RangeFor(iteratorVariable, oneConstant, null, input,
					new StaticCall(output, "times", new[] { output, iteratorVariable })
				));

			Assert.AreEqual(1.0, function(MFullArray<double>.CreateScalar(0)).ToScalar());
			Assert.AreEqual(1.0, function(MFullArray<double>.CreateScalar(1)).ToScalar());
			Assert.AreEqual(2.0, function(MFullArray<double>.CreateScalar(2)).ToScalar());
			Assert.AreEqual(120.0, function(MFullArray<double>.CreateScalar(5)).ToScalar());
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
		public void TestInout()
		{
			// function x = twice(x)
			//   x = x + x

			var variable = Declare<MArray<double>>("x");
			var function = CompileFunction<Func<MArray<double>, MArray<double>>>(
				variable, variable,
				new StaticCall(variable, "plus", new[] { variable, variable }));

			Assert.AreEqual(6.0, function(3).ToScalar());
		}
	}
}
