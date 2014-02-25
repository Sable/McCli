using McCli.Compiler.CodeGen;
using McCli.Compiler.IR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compiler.CommandLine
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine("Usage: {0} source [output]",
					Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location));
				return;
			}

			try
			{
				// Read the source file
				Console.WriteLine("Reading source file...");
				string sourceFilePath = args[0];
				CompilationUnit compilationUnit;
				using (var sourceFileStream = new FileStream(args[0], FileMode.Open, FileAccess.Read, FileShare.Read))
					compilationUnit = TamerXmlReader.Read(sourceFileStream);

				// Gather the referenced builtins
				var functionTable = new FunctionTable();
				functionTable.AddMethodsFromAssembly(typeof(Builtins.ArrayCreation).Assembly);

				// Output the dll.
				Console.WriteLine("Creating assembly builder...");
				string outputFilePath;
				if (args.Length >= 2)
				{
					outputFilePath = Path.GetFullPath(args[1]);
				}
				else
				{
					outputFilePath = Path.GetDirectoryName(sourceFilePath) + Path.DirectorySeparatorChar
						+ Path.GetFileNameWithoutExtension(sourceFilePath) + ".dll";
				}

				AssemblyEmitter.Emit(compilationUnit, outputFilePath, functionTable.Lookup);
			}
			catch (Exception exception)
			{
				Console.WriteLine("Compiler error: {0}", exception);
				if (Debugger.IsAttached) Debugger.Break();
			}
		}

		private static string MakeDefaultOutputFilePath(string sourceFilePath)
		{
			var directoryPath = Path.GetDirectoryName(sourceFilePath);
			var fileName = Path.GetFileName(sourceFilePath);
			fileName = fileName.Substring(0, fileName.IndexOf('.'));
			return directoryPath + Path.DirectorySeparatorChar + fileName + ".dll";
		}
	}
}
