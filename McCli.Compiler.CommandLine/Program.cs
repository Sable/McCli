﻿using McCli.Compiler.CodeGen;
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
			if (args.Length != 2)
			{
				Console.WriteLine("Usage: {0} {source} {output}",
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
				functionTable.AddStatic(typeof(Builtins.Operators));
				functionTable.AddStatic(typeof(Builtins.Classes));
				functionTable.AddStatic(typeof(Builtins.Complex));
				functionTable.AddStatic(typeof(Builtins.Environment));
				functionTable.AddStatic(typeof(Builtins.Floats));
				functionTable.AddStatic(typeof(Builtins.Arrays));

				// Output the dll.
				string outputFilePath = Path.GetFullPath(args[1]);
				string assemblyName = Path.GetFileNameWithoutExtension(outputFilePath);
				var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
					new AssemblyName(assemblyName),
					AssemblyBuilderAccess.Save,
					Path.GetDirectoryName(outputFilePath));
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
	}
}