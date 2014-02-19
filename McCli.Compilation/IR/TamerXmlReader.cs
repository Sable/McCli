using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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

			var variables = new Dictionary<string, Variable>();

			var variablesElement = functionElement.Element("Variables");
			if (variablesElement != null)
			{
				foreach (var variableElement in variablesElement.Elements("Variable"))
				{
					var variable = ReadVariable(variableElement);
					variables.Add(variable.Name, variable);
				}
			}

			throw new NotImplementedException();
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

			// Scalar, constant value
			throw new NotImplementedException();
		}
	}
}
