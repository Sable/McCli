using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compiler.IR
{
	/// <summary>
	/// Represents an unconditional jump, one of return, continue or break statements.
	/// </summary>
	public sealed class Jump : ControlFlow
	{
		public readonly JumpKind Kind;

		public Jump(JumpKind kind)
		{
			this.Kind = kind;
		}

		public override void Accept(Visitor visitor)
		{
			visitor.VisitJump(this);
		}
	}
}
