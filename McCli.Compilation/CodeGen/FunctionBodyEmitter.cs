using CliKit;
using McCli.Compilation.IR;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compilation.CodeGen
{
	public sealed class FunctionBodyEmitter : IR.Visitor
	{
		#region Fields
		private readonly IR.Function function;
		private readonly MethodInfo method;
		private readonly ILGenerator ilGenerator;
		private readonly Dictionary<Name, LocalLocation> locals = new Dictionary<Name, LocalLocation>();
		#endregion

		#region Constructors
		private FunctionBodyEmitter(IR.Function function, MethodFactory methodFactory)
		{
			Contract.Requires(function != null);
			Contract.Requires(methodFactory != null);

			this.function = function;

			var parameters = function.Inputs
				.Select(name => new ParameterDescriptor(typeof(MArray<double>), name.Value))
				.Concat(function.Outputs
					.Select(name => new ParameterDescriptor(typeof(MArray<double>).MakeByRefType(), name.Value))
				).ToImmutableArray();
			method = methodFactory(function.Name, parameters, ParameterDescriptor.VoidReturn, out ilGenerator);
		}
		#endregion

		#region Properties
		public MethodInfo Method
		{
			get { return method; }
		}
		#endregion

		#region Methods
		public void Emit()
		{
			foreach (var statement in function.Body)
				statement.Accept(this);
		}

		public override void VisitLiteral(Literal literal)
		{
			if (literal.Value is double)
			{
				ilGenerator.Emit(OpCodes.Ldc_R8, (double)literal.Value);
				var createDoubleScalarMethod = typeof(MArray<double>).GetMethod("CreateScalar");
				ilGenerator.Emit(OpCodes.Call, createDoubleScalarMethod);
			}
			else
			{
				base.VisitLiteral(literal);
			}
		}

		public override void VisitNode(IR.Node node)
		{
			throw new NotImplementedException();
		}

		private void EmitStore(Name name)
		{
			var location = GetLocalLocation(name);
		}

		private LocalLocation GetLocalLocation(Name name)
		{
			LocalLocation location;
			if (!locals.TryGetValue(name, out location))
			{
				var localBuilder = ilGenerator.DeclareLocal(typeof(MArray<double>));
				localBuilder.SetLocalSymInfo(name.Value);
				location = LocalLocation.Variable(localBuilder.LocalIndex);
				locals.Add(name, location);
			}

			return location;
		}
		#endregion
	}
}
