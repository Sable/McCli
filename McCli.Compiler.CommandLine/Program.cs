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
				Console.WriteLine("Reading source file...");
				string sourceFilePath = args[0];
				CompilationUnit compilationUnit;
				using (var sourceFileStream = new FileStream(args[0], FileMode.Open, FileAccess.Read, FileShare.Read))
					compilationUnit = TamerXmlReader.Read(sourceFileStream);

				// Gather the referenced builtins
				var functionTable = new FunctionTable();
				functionTable.AddMethodsFromAssembly(typeof(Builtins.Operators).Assembly);

				// Output the dll.
				Console.WriteLine("Creating assembly builder...");
				string outputFilePath = args.Length >= 2 ? Path.GetFullPath(args[1]) : MakeDefaultOutputFilePath(sourceFilePath);
				var assemblyName = Path.GetFileNameWithoutExtension(outputFilePath);

				// Create the assembly builder and friends
				var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
					new AssemblyName(assemblyName),
					AssemblyBuilderAccess.Save,
					Path.GetDirectoryName(outputFilePath));
				PortableClassLibrary.AddPortableFrameworkAttribute(assemblyBuilder);
				var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName, Path.GetFileName(outputFilePath), emitSymbolInfo: true);
				var typeBuilder = moduleBuilder.DefineType(assemblyName, TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class);

				// Emit the functions
				Console.WriteLine("Emitting function {0}...", compilationUnit.EntryPoint.Name);
				var methodFactory = MethodFactories.FromTypeBuilder(typeBuilder, MethodAttributes.Public | MethodAttributes.Static);
				FunctionBodyEmitter.Emit(compilationUnit.EntryPoint, methodFactory, functionTable.Lookup);

				// Save the assembly
				Console.WriteLine("Saving the assembly...");
				typeBuilder.CreateType();
				assemblyBuilder.Save(Path.GetFileName(outputFilePath));

				// Patch the assembly to make it a portable class library
				Console.WriteLine("Patching the generated assembly to make it a portable class library...");
				using (var assemblyStream = new FileStream(outputFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
					PortableClassLibrary.PatchReflectionEmitAssembly(assemblyStream);
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
	}
}
