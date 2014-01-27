using CliKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compilation.CodeGen
{
	public delegate MethodInfo MethodFactory(
		string name,
		ImmutableArray<ParameterDescriptor> parameters,
		ParameterDescriptor returnParameter,
		out ILGenerator ilGenerator);
}
