using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compiler.IR
{
	public sealed class Function : Node
	{
		#region Fields
		public readonly string Name;
		public readonly ImmutableArray<Variable> Inputs;
		public readonly ImmutableArray<Variable> Outputs;
		public readonly ImmutableArray<Statement> Body;
		#endregion

		#region Constructors
		public Function(string name, ImmutableArray<Variable> inputs, ImmutableArray<Variable> outputs, ImmutableArray<Statement> body)
		{
			Contract.Requires(name != null);

			this.Name = name;
			this.Inputs = inputs;
			this.Outputs = outputs;
			this.Body = body;
		}
		#endregion

		#region Properties
		#endregion

		#region Methods
		public override void Accept(Visitor visitor)
		{
			visitor.VisitFunction(this);
		}

		public override string ToDebugString()
		{
			string format = "function ";
			if (Outputs.Length > 0) format += "[{0}] = ";
			format += "{1}({2}) [{3} statements]";
			return string.Format(format, CommaSeparate(Outputs), Name, CommaSeparate(Inputs), Body.Length);
		}
		#endregion
	}
}
