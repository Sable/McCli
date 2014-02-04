using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compilation
{
	public sealed class FunctionInfo
	{
		#region Fields
		private readonly MethodInfo method;
		private readonly ImmutableArray<MType> inputTypes;
		private readonly MType outputType;
		#endregion

		#region Constructors
		public FunctionInfo(MethodInfo method)
		{
			Contract.Requires(method != null);

			this.method = method;
			inputTypes = method.GetParameters().Select(p => MType.FromCliType(p.ParameterType)).ToImmutableArray();
			outputType = MType.FromCliType(method.ReturnType);
		}
		#endregion

		#region Properties
		public MethodInfo Method
		{
			get { return method; }
		}

		public ImmutableArray<MType> InputTypes
		{
			get { return inputTypes; }
		}

		public MType OutputType
		{
			get { return outputType; }
		}
		#endregion

		#region Methods
		public override string ToString()
		{
			var stringBuilder = new StringBuilder();

			stringBuilder.Append(method.Name).Append(" : (");
			for (int i = 0; i < inputTypes.Length; ++i)
			{
				if (i > 0) stringBuilder.Append(", ");
				stringBuilder.Append(inputTypes[i]);
			}

			stringBuilder.Append(") -> ").Append(outputType);

			return stringBuilder.ToString();
		}
		#endregion
	}
}
