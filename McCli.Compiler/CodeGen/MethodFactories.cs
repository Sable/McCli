using CliKit;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compiler.CodeGen
{
	public static class MethodFactories
	{
		public static DynamicMethod Dynamic(string name,
			ImmutableArray<ParameterDescriptor> parameters, Type returnType,
			out ILGenerator ilGenerator)
		{
			Contract.Requires(name != null);

			var method = new DynamicMethod(name, returnType, GetTypeArray(parameters));

			// Define the input/output parameters
			for (int i = 0; i < parameters.Length; ++i)
				method.DefineParameter(i + 1, parameters[i].Attributes, parameters[i].Name ?? string.Empty);
			
			ilGenerator = method.GetILGenerator();

			return method;
		}

		public static MethodFactory FromTypeBuilder(TypeBuilder typeBuilder, MethodAttributes attributes)
		{
			Contract.Requires(typeBuilder != null);
			Contract.Requires((attributes & MethodAttributes.Static) == MethodAttributes.Static);

			return delegate(string name, ImmutableArray<ParameterDescriptor> parameters,
				Type returnType, out ILGenerator ilGenerator)
			{
				var methodBuilder = typeBuilder.DefineMethod(name, attributes, returnType, GetTypeArray(parameters));

				for (int i = 0; i < parameters.Length; ++i)
					methodBuilder.DefineParameter(i + 1, parameters[i].Attributes, parameters[i].Name ?? string.Empty);

				ilGenerator = methodBuilder.GetILGenerator();

				return methodBuilder;
			};
		}

		private static Type[] GetTypeArray(ImmutableArray<ParameterDescriptor> parameters)
		{
			var parameterTypes = new Type[parameters.Length];
			for (int i = 0; i < parameters.Length; ++i)
				parameterTypes[i] = parameters[i].Type;
			return parameterTypes;
		}
	}
}
