using McCli.Compiler.IR;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compiler.CodeGen
{
	public static class AssemblyEmitter
	{
		public static void Emit(CompilationUnit compilationUnit, string filePath, FunctionLookup builtinLookup)
		{
			Contract.Requires(compilationUnit != null);
			Contract.Requires(filePath != null);
			Contract.Requires(builtinLookup != null);

			var assemblyName = Path.GetFileNameWithoutExtension(filePath);

			// Create the assembly builder and friends
			var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
				new AssemblyName(assemblyName),
				AssemblyBuilderAccess.Save,
				Path.GetDirectoryName(filePath));
			PortableClassLibrary.AddPortableFrameworkAttribute(assemblyBuilder);
			var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName, Path.GetFileName(filePath), emitSymbolInfo: true);
			var typeBuilder = moduleBuilder.DefineType(assemblyName, TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class);
			
			// Create the function emitters to "declare" the methods
			var functionEmitters = new Dictionary<Function, FunctionEmitter>();
			FunctionLookup functionLookup = (name, inputReprs) =>
			{
				// TODO: Return a function emitter's method if needed
				return builtinLookup(name, inputReprs);
			};

			var methodFactory = MethodFactories.FromTypeBuilder(typeBuilder, MethodAttributes.Public | MethodAttributes.Static);
			foreach (var function in compilationUnit.Functions.Values)
				functionEmitters.Add(function, new FunctionEmitter(function, methodFactory, functionLookup));

			foreach (var functionEmitter in functionEmitters.Values)
				functionEmitter.EmitBody();

			// Save the assembly
			typeBuilder.CreateType();
			assemblyBuilder.Save(Path.GetFileName(filePath));

			// Patch the assembly to make it a portable class library
			Console.WriteLine("Patching the generated assembly to make it a portable class library...");
			using (var assemblyStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
				PortableClassLibrary.PatchReflectionEmitAssembly(assemblyStream);
		}
	}
}
