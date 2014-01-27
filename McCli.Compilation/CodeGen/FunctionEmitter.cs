using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compilation.CodeGen
{
	public sealed class FunctionEmitter : IR.Visitor
	{
		#region Fields
		private readonly IR.Function function;
		private readonly MethodBuilder methodBuilder;
		#endregion

		#region Constructors
		private FunctionEmitter(IR.Function function, MethodBuilder methodBuilder)
		{
			Contract.Requires(function != null);
			Contract.Requires(methodBuilder != null);

			this.function = function;
			this.methodBuilder = methodBuilder;
		}
		#endregion

		#region Properties
		#endregion

		#region Methods
		public void Emit()
		{
			function.Accept(this);
		}

		public override void VisitNode(IR.Node node)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
