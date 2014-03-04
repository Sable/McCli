using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compiler
{
	/// <summary>
	/// Represents the signature of a MatLab function implemented as a .NET method.
	/// </summary>
	public sealed class FunctionSignature
	{
		#region Fields
		private readonly List<MRepr> inputs = new List<MRepr>();
		private readonly List<MRepr> outputs = new List<MRepr>();
		#endregion

		#region Constructors
		public FunctionSignature(IEnumerable<MRepr> inputs, IEnumerable<MRepr> outputs)
		{
			Contract.Requires(inputs != null);
			Contract.Requires(outputs != null);

			this.inputs = inputs.ToList();
			this.outputs = outputs.ToList();
		}

		public FunctionSignature(ParameterInfo[] parameters, Type returnType)
		{
			Contract.Requires(parameters != null);
			Contract.Requires(returnType != null);

			foreach (var parameter in parameters)
			{
				var parameterType = parameter.ParameterType;
				if (parameterType.IsByRef)
				{
					Contract.Assert(parameter.IsOut);
					outputs.Add(MRepr.FromCliType(parameterType.GetElementType()));
				}
				else
				{
					Contract.Assert(outputs.Count == 0);
					inputs.Add(MRepr.FromCliType(parameterType));
				}
			}

			Contract.Assert(outputs.Count != 1);

			if (returnType != typeof(void))
			{
				Contract.Assert(outputs.Count == 0);
				outputs.Add(MRepr.FromCliType(returnType));
			}
		}
		#endregion

		#region Properties
		public IReadOnlyList<MRepr> Inputs
		{
			get { return inputs; }
		}

		public IReadOnlyList<MRepr> Outputs
		{
			get { return outputs; }
		}

		public bool HasReturnValue
		{
			get { return outputs.Count == 1; }
		}

		public bool UsesOutParameters
		{
			get { return outputs.Count >= 2; }
		}

		public int OutParameterCount
		{
			get { return outputs.Count >= 2 ? outputs.Count : 0; }
		}

		public Type ReturnCliType
		{
			get { return outputs.Count == 1 ? outputs[0].CliType : typeof(void); }
		}
		#endregion

		#region Methods
		public Type[] GetParameterCliTypes()
		{
			var outParameterCount = OutParameterCount;
			var types = new Type[inputs.Count + outParameterCount];
			for (int i = 0; i < inputs.Count; ++i) types[i] = inputs[i].CliType;
			for (int i = 0; i < outParameterCount; ++i) types[inputs.Count + i] = outputs[i].CliType.MakeByRefType();
			return types;
		}

		public override string ToString()
		{
			var stringBuilder = new StringBuilder();

			stringBuilder.Append('(');
			bool needsComma = false;
			foreach (var input in inputs)
			{
				if (needsComma) stringBuilder.Append(", ");
				stringBuilder.Append(input);
				needsComma = true;
			}

			if (OutParameterCount > 0)
			{
				foreach (var output in outputs)
				{
					if (needsComma) stringBuilder.Append(", ");
					stringBuilder.Append("out ").Append(output);
					needsComma = true;
				}
			}

			stringBuilder.Append(") -> ").Append(HasReturnValue ? outputs[0].ToString() : "()");

			return stringBuilder.ToString();
		}
		#endregion
	}
}
