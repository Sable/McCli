using McCli.Compiler.CodeGen;
using McCli.Compiler.IR;
using System;
using System.Collections.Generic;
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
				string sourceFilePath = args[0];
				Function entryPointFunction;
				using (var sourceFileStream = new FileStream(args[0], FileMode.Open, FileAccess.Read, FileShare.Read))
					entryPointFunction = TamerXmlReader.Read(sourceFileStream).Single();

				// Gather the referenced builtins
				var functionTable = new FunctionTable();
				functionTable.AddMethodsFromAssembly(typeof(Builtins.Operators).Assembly);

				// Output the dll.
				string outputFilePath = args.Length >= 2 ? Path.GetFullPath(args[1]) : MakeDefaultOutputFilePath(sourceFilePath);
				var assemblyName = MakeAssemblyName(outputFilePath);
				var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
					new AssemblyName(assemblyName),
					AssemblyBuilderAccess.Save,
					Path.GetDirectoryName(outputFilePath));
				PortableClassLibrary.AddPortableFrameworkAttribute(assemblyBuilder);
				var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName, Path.GetFileName(outputFilePath), emitSymbolInfo: true);
				var typeBuilder = moduleBuilder.DefineType(assemblyName, TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class);
				var methodFactory = MethodFactories.FromTypeBuilder(typeBuilder, MethodAttributes.Public | MethodAttributes.Static);
				FunctionBodyEmitter.Emit(entryPointFunction, methodFactory, functionTable.Lookup);
				typeBuilder.CreateType();
				assemblyBuilder.Save(Path.GetFileName(outputFilePath));
			}
			catch (Exception exception)
			{
				Console.WriteLine("Compiler error: {0}", exception);
			}
		}

		private static string MakeDefaultOutputFilePath(string sourceFilePath)
		{
			var directoryPath = Path.GetDirectoryName(sourceFilePath);
			var fileName = Path.GetFileName(sourceFilePath);
			fileName = fileName.Substring(0, fileName.IndexOf('.'));
			return directoryPath + Path.DirectorySeparatorChar + fileName + ".dll";
		}

		private static string MakeAssemblyName(string path)
		{
			path = Path.GetFileName(path);
			return path.Substring(0, path.IndexOf('.'));
		}
	}
}
