using CliKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compiler.CodeGen
{
	public delegate MethodInfo MethodFactory(string name,
		IReadOnlyList<ParameterDescriptor> parameters, Type returnType,
		out ILGenerator ilGenerator);
}
