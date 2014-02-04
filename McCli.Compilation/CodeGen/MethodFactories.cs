using CliKit;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compilation.CodeGen
{
	public static class MethodFactories
	{
		public static DynamicMethod Dynamic(string name,
			ImmutableArray<ParameterDescriptor> parameters, Type returnType,
			out ILGenerator ilGenerator)
		{
			Contract.Requires(name != null);

			var parameterTypes = new Type[parameters.Length];
			for (int i = 0; i < parameters.Length; ++i)
				parameterTypes[i] = parameters[i].Type;

			var method = new DynamicMethod(name, returnType, parameterTypes);

			// Define the input/output parameters
			for (int i = 0; i < parameters.Length; ++i)
				method.DefineParameter(i + 1, parameters[i].Attributes, parameters[i].Name ?? string.Empty);
			
			ilGenerator = method.GetILGenerator();

			return method;
		}
	}
}
