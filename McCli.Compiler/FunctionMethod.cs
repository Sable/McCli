using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compiler
{
	public sealed class FunctionMethod
	{
		#region Fields
		private readonly MethodInfo method;
		private readonly ImmutableArray<MRepr> inputReprs;
		private readonly MRepr outputType;
		#endregion

		#region Constructors
		public FunctionMethod(MethodInfo method)
		{
			Contract.Requires(method != null);

			this.method = method;
			inputReprs = method.GetParameters().Select(p => MRepr.FromCliType(p.ParameterType)).ToImmutableArray();
			outputType = MRepr.FromCliType(method.ReturnType);
		}

		public FunctionMethod(MethodInfo method, Type[] inputTypes, Type returnType)
		{
			Contract.Requires(method != null);
			Contract.Requires(inputTypes != null);
			Contract.Requires(returnType != null);

			this.method = method;
			inputReprs = inputTypes.Select(t => MRepr.FromCliType(t)).ToImmutableArray();
			outputType = MRepr.FromCliType(returnType);
		}
		#endregion

		#region Properties
		public string Name
		{
			get { return method.Name; }
		}

		public MethodInfo Method
		{
			get { return method; }
		}

		public ImmutableArray<MRepr> InputReprs
		{
			get { return inputReprs; }
		}

		public MRepr OutputRepr
		{
			get { return outputType; }
		}
		#endregion

		#region Methods
		public override string ToString()
		{
			var stringBuilder = new StringBuilder();

			stringBuilder.Append(method.Name).Append(" : (");
			for (int i = 0; i < inputReprs.Length; ++i)
			{
				if (i > 0) stringBuilder.Append(", ");
				stringBuilder.Append(inputReprs[i]);
			}

			stringBuilder.Append(") -> ").Append(outputType);

			return stringBuilder.ToString();
		}
		#endregion
	}
}
