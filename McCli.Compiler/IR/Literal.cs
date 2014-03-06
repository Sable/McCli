using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compiler.IR
{
	/// <summary>
	/// Represents an expression which assigns a literal value to a variable.
	/// </summary>
	public sealed class Literal : Expression
	{
		#region Fields
		public readonly Variable Target;
		public readonly object Value;
		#endregion

		#region Constructors
		public Literal(Variable target, double value)
		{
			Contract.Requires(target != null);

			this.Target = target;
			this.Value = value;
		}

		public Literal(Variable target, string str)
		{
			Contract.Requires(target != null);
			Contract.Requires(str != null);

			this.Target = target;
			this.Value = str;
		}
		#endregion

		#region Properties
		public LiteralType Type
		{
			get
			{
				if (Value is double) return LiteralType.Double;
				if (Value is string) return LiteralType.String;
				throw new InvalidOperationException("Unexpected literal type.");
			}
		}

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
			visitor.VisitLiteral(this);
		}

		public override string ToDebugString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{0} = " + (Value is string || Value is char ? "'{1}'" : "{1}"), Target.Name, Value);
		}
		#endregion
	}
}
