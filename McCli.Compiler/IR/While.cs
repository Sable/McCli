﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compiler.IR
{
	/// <summary>
	/// Represents a MatLab while loop.
	/// </summary>
	public sealed class While : ControlFlow
	{
		public readonly Variable Condition;
		public readonly ImmutableArray<Statement> Body;

		public While(Variable condition, ImmutableArray<Statement> body)
		{
			Contract.Requires(condition != null);
			this.Condition = condition;
			this.Body = body;
		}

		public While(Variable condition, params Statement[] body)
			: this(condition, ImmutableArray.Create(body)) { }

		public override void Accept(Visitor visitor)
		{
			visitor.VisitWhile(this);
		}

		public override string ToDebugString()
		{
			return string.Format("while ({0}) [{1} statements]", Condition.Name, Body.Length);
		}
	}
}
