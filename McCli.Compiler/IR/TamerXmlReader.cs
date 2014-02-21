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
		public static IEnumerable<Function> Read(Stream stream)
		{
			Contract.Requires(stream != null && stream.CanRead);

			var document = XDocument.Load(stream);
			var root = document.Root;
			Contract.Assert(root != null && root.Name == "CompilationUnit");

			foreach (var functionElement in root.Elements("Function"))
				yield return ReadFunction(functionElement);
		}

		private static Function ReadFunction(XElement functionElement)
		{
			Contract.Requires(functionElement != null && functionElement.Name == "Function");

			string name = (string)functionElement.Attribute("name");

			// "Declare" variables
			var variables = new Dictionary<string, Variable>(StringComparer.Ordinal);
			var inputs = new List<Variable>();
			var outputs = new List<Variable>();
			var variablesElement = functionElement.Element("Variables");
			if (variablesElement != null)
			{
				foreach (var variableElement in variablesElement.Elements("Variable"))
				{
					var variable = ReadVariable(variableElement);
					variables.Add(variable.Name, variable);

					if (variable.Kind == VariableKind.Input) inputs.Add(variable);
					else if (variable.Kind == VariableKind.Output) outputs.Add(variable);
				}
			}

			// Read the function body
			var body = ReadStatements(functionElement.Element("Statements"), variables).ToImmutableArray();

			return new Function(name, inputs.ToImmutableArray(), outputs.ToImmutableArray(), body);
		}

		private static Variable ReadVariable(XElement variableElement)
		{
			var name = (string)variableElement.Attribute("name");

			MType type = null;
			var className = (string)variableElement.Attribute("class");
			var complex = (bool?)variableElement.Attribute("complex");
			if (className != null && complex.HasValue)
			{
				type = MClass.FromName((string)className);
				if (complex.Value) type = ((MPrimitiveClass)type).Complex;
			}

			bool scalar = ((bool?)variableElement.Attribute("scalar")).GetValueOrDefault();

			string constantString = (string)variableElement.Attribute("constant");
			object constantValue = null;
			if (constantString != null)
			{
				if (type == MClass.Double)
					constantValue = double.Parse(constantString, CultureInfo.InvariantCulture);
			}

			var kind = (VariableKind)Enum.Parse(typeof(VariableKind),
				(string)variableElement.Attribute("scope"), ignoreCase: true);

			return constantValue == null
				? new Variable(name, kind, new MRepr(type, scalar ? MStructuralClass.Scalar : MStructuralClass.Array))
				: new Variable(name, kind, constantValue);
		}

		private static IEnumerable<Statement> ReadStatements(XElement parentElement, IReadOnlyDictionary<string, Variable> variables)
		{
			if (parentElement == null) yield break;
			foreach (var statementElement in parentElement.Elements())
			{
				switch (statementElement.Name.LocalName)
				{
					case "ArrayGet": yield return ReadArrayGet(statementElement, variables); break;
					case "ArraySet": yield return ReadArraySet(statementElement, variables); break;
					case "Call": yield return ReadCall(statementElement, variables); break;
					case "Copy": yield return ReadCopy(statementElement, variables); break;
					case "For": yield return ReadFor(statementElement, variables); break;
					case "Literal": yield return ReadLiteral(statementElement, variables); break;
					default: throw new NotImplementedException("Statements of type: " + statementElement.Name.LocalName);
				}
			}
		}

		private static LoadParenthesized ReadArrayGet(XElement statementElement, IReadOnlyDictionary<string, Variable> variables)
		{
			var targets = ReadVariables(statementElement.Attribute("targets"), variables).ToImmutableArray();
			var subject = ReadVariable(statementElement.Attribute("subject"), variables);
			var arguments = ReadVariables(statementElement.Attribute("indices"), variables).ToImmutableArray();
			return new LoadParenthesized(targets, subject, arguments);
		}

		private static StoreParenthesized ReadArraySet(XElement statementElement, IReadOnlyDictionary<string, Variable> variables)
		{
			var array = ReadVariable(statementElement.Attribute("array"), variables);
			var indices = ReadVariables(statementElement.Attribute("indices"), variables).ToImmutableArray();
			var value = ReadVariable(statementElement.Attribute("value"), variables);
			return new StoreParenthesized(array, indices, value);
		}

		private static StaticCall ReadCall(XElement statementElement, IReadOnlyDictionary<string, Variable> variables)
		{
			var functionName = (string)statementElement.Attribute("functionName");
			var targets = ReadVariables(statementElement.Attribute("targets"), variables).ToImmutableArray();
			var arguments = ReadVariables(statementElement.Attribute("arguments"), variables).ToImmutableArray();

			return new StaticCall(targets, functionName, arguments);
		}

		private static Copy ReadCopy(XElement statementElement, IReadOnlyDictionary<string, Variable> variables)
		{
			var target = ReadVariable(statementElement.Attribute("target"), variables);
			var value = ReadVariable(statementElement.Attribute("source"), variables);
			return new Copy(target, value);
		}

		private static RangeFor ReadFor(XElement statementElement, IReadOnlyDictionary<string, Variable> variables)
		{
			var iterator = ReadVariable(statementElement.Attribute("iterator"), variables);
			var lowerBound = ReadVariable(statementElement.Attribute("lowerBound"), variables);
			var upperBound = ReadVariable(statementElement.Attribute("upperBound"), variables);
			var increment = ReadVariable(statementElement.Attribute("increment"), variables);
			var body = ReadStatements(statementElement, variables).ToImmutableArray();
			return new RangeFor(iterator, lowerBound, increment, upperBound, body);
		}

		private static Literal ReadLiteral(XElement statementElement, IReadOnlyDictionary<string, Variable> variables)
		{
			var target = ReadVariable(statementElement.Attribute("target"), variables);
			string valueString = (string)statementElement.Attribute("value");
			switch ((string)statementElement.Attribute("type"))
			{
				case "double": return new Literal(target, double.Parse(valueString, CultureInfo.InvariantCulture));
				case "char": return new Literal(target, valueString[0]);
				default: throw new InvalidDataException("Unexpected literal type.");
			}
		}

		private static Variable ReadVariable(XAttribute attribute, IReadOnlyDictionary<string, Variable> variables)
		{
			if (attribute == null) return null;

			Variable variable;
			variables.TryGetValue(attribute.Value, out variable);
			return variable;
		}

		private static IEnumerable<Variable> ReadVariables(XAttribute attribute, IReadOnlyDictionary<string, Variable> variables)
		{
			if (attribute == null) yield break;

			string commaSeparatedList = attribute.Value;
			if (commaSeparatedList.Length == 0) yield break;

			foreach (var name in commaSeparatedList.Split(new[] { ',' }))
				yield return variables[name];
		}
	}
}
