using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compiler.CodeGen
{
	public delegate FunctionMethod FunctionLookup(string name, ImmutableArray<MRepr> inputReprs);
}
