using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace McCli.Compiler.IR
{
	/// <summary>
	/// Reads Tamer IR from its XML-serialized representation.
	/// </summary>
	public sealed class TamerXmlReader
	{
		public static CompilationUnit Read(Stream stream)
		{
			Contract.Requires(stream != null && stream.CanRead);

			var document = XDocument.Load(stream);
			var root = document.Root;
			Contract.Assert(root != null && root.Name == "CompilationUnit");

			var functions = new List<Function>();
			Function entryPoint = null;
			foreach (var element in root.Elements("Function"))
			{
				bool isEntryPoint;
				var function = ReadFunction(element, out isEntryPoint);

				if (isEntryPoint)
				{
					if (entryPoint != null) throw new InvalidDataException("Compilation unit has multiple entry point.");
					entryPoint = function;
				}

				functions.Add(function);
			}

			if (entryPoint == null) throw new InvalidDataException("Compilation unit has no entry point.");
			return new CompilationUnit(functions, entryPoint);
		}

		private static Function ReadFunction(XElement functionElement, out bool entryPoint)
		{
			Contract.Requires(functionElement != null && functionElement.Name == "Function");

			string name = (string)functionElement.Attribute("name");
			entryPoint = ((bool?)functionElement.Attribute("entry")).GetValueOrDefault();

			// "Declare" variables
			var variables = new Dictionary<string, Variable>(StringComparer.Ordinal);
			var variablesElement = functionElement.Element("Variables");
			if (variablesElement != null)
			{
				foreach (var variableElement in variablesElement.Elements("Variable"))
				{
					var variable = ReadVariable(variableElement);
					variables.Add(variable.Name, variable);
				}
			}

			var inputs = ReadVariables(functionElement.Attribute("inputs"), variables);
			var outputs = ReadVariables(functionElement.Attribute("outputs"), variables);

			// Read the function body
			var body = ReadStatements(functionElement.Element("Body"), variables).ToImmutableArray();

			return new Function(name, inputs, outputs, body);
		}

		private static Variable ReadVariable(XElement variableElement)
		{
			var name = (string)variableElement.Attribute("name");

			MType type = null;
			var className = (string)variableElement.Attribute("class");
			if (className != null)
			{
				type = MClass.FromName((string)className);
				if (type.IsPrimitive)
				{
					// If we don't have complex information, then the variable's value
					// is sometimes real and sometimes complex, which we approximate
					// by making it always complex, though this can be incorrect.
					var complex = (bool?)variableElement.Attribute("complex") ?? true;
					if (complex) type = ((MPrimitiveClass)type).Complex;
				}
			}

			bool scalar = ((bool?)variableElement.Attribute("scalar")).GetValueOrDefault();
			bool initOnly = ((bool?)variableElement.Attribute("final")).GetValueOrDefault();

			string constantString = (string)variableElement.Attribute("constant");
			object constantValue = null;
			if (constantString != null)
			{
				if (type == MClass.Double)
					constantValue = double.Parse(constantString, CultureInfo.InvariantCulture);
				// TODO: Implement strings
			}

			return constantValue == null
				? new Variable(name, new MRepr(type, scalar ? MStructuralClass.Scalar : MStructuralClass.Array), initOnly)
				: new Variable(name, constantValue, initOnly);
		}

		private static IEnumerable<Statement> ReadStatements(XElement parentElement, IReadOnlyDictionary<string, Variable> variables)
		{
			if (parentElement == null) yield break;
			foreach (var element in parentElement.Elements())
			{
				switch (element.Name.LocalName)
				{
					case "ArrayGet": yield return ReadArrayGet(element, variables); break;
					case "ArraySet": yield return ReadArraySet(element, variables); break;
					case "Break": yield return new Jump(JumpKind.Break); break;
					case "Call": yield return ReadCall(element, variables); break;
					case "Continue": yield return new Jump(JumpKind.Continue); break;
					case "Copy": yield return ReadCopy(element, variables); break;
					case "For": yield return ReadFor(element, variables); break;
					case "If": yield return ReadIf(element, variables); break;
					case "Literal": yield return ReadLiteral(element, variables); break;
					case "Return": yield return new Jump(JumpKind.Return); break;
					case "While": yield return ReadWhile(element, variables); break;
					default: throw new NotImplementedException("Statements of type: " + element.Name.LocalName);
				}
			}
		}

		private static LoadParenthesized ReadArrayGet(XElement element, IReadOnlyDictionary<string, Variable> variables)
		{
			var targets = ReadVariables(element.Attribute("targets"), variables);
			var subject = ReadVariable(element.Attribute("array"), variables);
			var arguments = ReadVariables(element.Attribute("indices"), variables);
			return new LoadParenthesized(targets, subject, arguments);
		}

		private static StoreParenthesized ReadArraySet(XElement element, IReadOnlyDictionary<string, Variable> variables)
		{
			var array = ReadVariable(element.Attribute("array"), variables);
			var indices = ReadVariables(element.Attribute("indices"), variables);
			var value = ReadVariable(element.Attribute("value"), variables);
			return new StoreParenthesized(array, indices, value);
		}

		private static StaticCall ReadCall(XElement element, IReadOnlyDictionary<string, Variable> variables)
		{
			var functionName = (string)element.Attribute("function");
			var targets = ReadVariables(element.Attribute("targets"), variables);
			var arguments = ReadVariables(element.Attribute("arguments"), variables);

			return new StaticCall(targets, functionName, arguments);
		}

		private static Copy ReadCopy(XElement element, IReadOnlyDictionary<string, Variable> variables)
		{
			var target = ReadVariable(element.Attribute("target"), variables);
			var value = ReadVariable(element.Attribute("source"), variables);
			return new Copy(target, value);
		}

		private static RangeFor ReadFor(XElement element, IReadOnlyDictionary<string, Variable> variables)
		{
			var iterator = ReadVariable(element.Attribute("iterator"), variables);
			var from = ReadVariable(element.Attribute("from"), variables);
			var to = ReadVariable(element.Attribute("to"), variables);
			var step = ReadVariable(element.Attribute("step"), variables);
			var body = ReadStatements(element, variables).ToImmutableArray();
			return new RangeFor(iterator, from, step, to, body);
		}

		private static If ReadIf(XElement element, IReadOnlyDictionary<string, Variable> variables)
		{
			var condition = ReadVariable(element.Attribute("condition"), variables);
			var then = ReadStatements(element.Element("Then"), variables).ToImmutableArray();
			var @else = ReadStatements(element.Element("Else"), variables).ToImmutableArray();
			return new If(condition, then, @else);
		}

		private static Literal ReadLiteral(XElement element, IReadOnlyDictionary<string, Variable> variables)
		{
			var target = ReadVariable(element.Attribute("target"), variables);
			string valueString = (string)element.Attribute("value");
			switch ((string)element.Attribute("type"))
			{
				case "double": return new Literal(target, double.Parse(valueString, CultureInfo.InvariantCulture));
				case "char": return new Literal(target, valueString);
				default: throw new InvalidDataException("Unexpected literal type.");
			}
		}

		private static While ReadWhile(XElement element, IReadOnlyDictionary<string, Variable> variables)
		{
			var condition = ReadVariable(element.Attribute("condition"), variables);
			var body = ReadStatements(element, variables).ToImmutableArray();
			return new While(condition, body);
		}

		private static Variable ReadVariable(XAttribute attribute, IReadOnlyDictionary<string, Variable> variables)
		{
			if (attribute == null) return null;
			return ResolveVariable(attribute.Value, variables);
		}

		private static ImmutableArray<Variable> ReadVariables(XAttribute attribute, IReadOnlyDictionary<string, Variable> lookup)
		{
			if (attribute == null) return ImmutableArray.Empty;

			string commaSeparatedList = attribute.Value;
			if (commaSeparatedList.Length == 0) return ImmutableArray.Empty;

			var variables = new List<Variable>();
			foreach (var name in commaSeparatedList.Split(new[] { ',' }))
				variables.Add(ResolveVariable(name, lookup));
			return variables.ToImmutableArray();
		}

		private static Variable ResolveVariable(string name, IReadOnlyDictionary<string, Variable> lookup)
		{
			Variable variable;
			if (!lookup.TryGetValue(name, out variable))
			{
				string message = string.Format(CultureInfo.InvariantCulture, "Reference to undefined variable '{0}'.", name);
				throw new KeyNotFoundException(message);
			}

			return variable;
		}
	}
}
