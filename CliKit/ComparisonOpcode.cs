using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emit = System.Reflection.Emit;

namespace CliKit
{
	/// <summary>
	/// Represents IL opcodes which compare two values.
	/// </summary>
	public sealed class ComparisonOpcode : Opcode
	{
		#region Fields
		private readonly Comparison comparison;
		#endregion

		#region Constructors
		internal ComparisonOpcode(Emit.OpCode opcode, Comparison comparison) : base(opcode)
		{
			this.comparison = comparison;
		}
		#endregion

		#region Properties
		public Comparison Comparison
		{
			get { return comparison; }
		}
		#endregion

		#region Methods
		public override void Accept<T>(OpcodeVisitor<T> visitor, T param)
		{
			visitor.VisitComparison(this, param);
		}
		#endregion
	}
}
