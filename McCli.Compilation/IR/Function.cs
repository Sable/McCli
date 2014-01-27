using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compilation.IR
{
	public sealed class Function : Node
	{
		#region Fields
		public readonly Name Name;
		public readonly ImmutableArray<Name> Inputs;
		public readonly ImmutableArray<Name> Outputs;
		public readonly ImmutableArray<Statement> Body;
		#endregion

		#region Constructors
		public Function(Name name, ImmutableArray<Name> inputs, ImmutableArray<Name> outputs, ImmutableArray<Statement> body)
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
		#endregion
	}
}
