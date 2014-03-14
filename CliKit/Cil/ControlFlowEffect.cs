using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliKit.Cil
{
	/// <summary>
	/// Indicates the effect of an instruction on the control flow.
	/// </summary>
	public enum ControlFlowEffect
	{
		UnconditionalBranch = 0,
		Break = 1,
		Call = 2,
		ConditionalBranch = 3,
		Prefix = 4,
		Normal = 5,
		Return = 7,
		Throw = 8,
	}
}
