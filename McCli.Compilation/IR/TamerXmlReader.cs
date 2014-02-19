using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace McCli.Compilation.IR
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

			foreach (var functionElement in root.Elements("TIRFunction"))
				yield return ReadFunction(functionElement);
		}

		private static Function ReadFunction(XElement functionElement)
		{
			Contract.Requires(functionElement != null && functionElement.Name == "TIRFunction");

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
			var body = ReadStatements(functionElement.Element("TIRStatementList"), variables).ToImmutableArray();

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

		private static IEnumerable<Statement> ReadStatements(XElement statementListElement, IReadOnlyDictionary<string, Variable> variables)
		{
			if (statementListElement == null) yield break;
			foreach (var statementElement in statementListElement.Elements())
			{
				switch (statementElement.Name.LocalName)
				{
					case "TIRAssignLiteralStmt": yield return ReadLiteral(statementElement, variables); break;
					case "TIRCallStmt": yield return ReadCall(statementElement, variables); break;
					default: throw new NotImplementedException();
				}
			}
		}

		private static Literal ReadLiteral(XElement statementElement, IReadOnlyDictionary<string, Variable> variables)
		{
			var target = variables[(string)statementElement.Attribute("targetName")];
			string valueString = (string)statementElement.Attribute("value");
			switch ((string)statementElement.Attribute("type"))
			{
				case "double": return new Literal(target, double.Parse(valueString, CultureInfo.InvariantCulture));
				case "char": return new Literal(target, valueString[0]);
				default: throw new InvalidDataException("Unexpected literal type.");
			}
		}

		private static StaticCall ReadCall(XElement statementElement, IReadOnlyDictionary<string, Variable> variables)
		{
			string functionName = (string)statementElement.Attribute("functionName");

			var targets = ReadVariableNames(statementElement.Element("Targets"), variables).ToImmutableArray();
			var arguments = ReadVariableNames(statementElement.Element("Arguments"), variables).ToImmutableArray();

			return new StaticCall(targets, functionName, arguments);
		}

		private static IEnumerable<Variable> ReadVariableNames(XElement parentElement, IReadOnlyDictionary<string, Variable> variables)
		{
			if (parentElement == null) yield break;
			foreach (var nameElement in parentElement.Elements("Name"))
			{
				var name = (string)nameElement.Attribute("nameId");
				yield return variables[name];
			}
		}
	}
}
