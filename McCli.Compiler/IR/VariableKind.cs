using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compiler.IR
{
	public enum VariableKind
	{
		Local,
		Input,
		Output,
		Global,
		Persistent
	}
}
