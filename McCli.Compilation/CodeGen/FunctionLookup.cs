using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compilation.CodeGen
{
	public delegate MethodInfo FunctionLookup(string name, ImmutableArray<MClass> argumentClasses);
}
