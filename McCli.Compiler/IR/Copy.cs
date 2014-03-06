using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compiler.IR
{
	/// <summary>
	/// IR node where the value of a variable is assigned to another: <c>target = value</c>.
	/// This only differs from <see cref="LoadParenthesized"/> with zero arguments if the value is a function handle.
	/// </summary>
	public sealed class Copy : Expression
	{
		#region Fields
		public readonly Variable Target, Value;
		#endregion

		#region Constructors
		public Copy(Variable target, Variable value)
		{
			Contract.Requires(target != null);
			Contract.Requires(value != null);

			this.Target = target;
			this.Value = value;
		}
		#endregion

		#region Properties
		public override int TargetCount
		{
			get { return 1; }
		}
		#endregion

		#region Methods
		public override Variable GetTarget(int index)
		{
			return Target;
		}

		public override void Accept(Visitor visitor)
		{
			visitor.VisitCopy(this);
		}

		public override string ToDebugString()
		{
			return string.Format("{0} = {1}", Target.Name, Value.Name);
		}
		#endregion
	}
}
