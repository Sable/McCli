using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliKit.Cil
{
	public enum CallKind
	{
		EarlyBound,
		Virtual,
		Indirect,
		Jump,
		Constructor
	}
}
