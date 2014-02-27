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
				MRepr repr = MRepr.FromCliType(parameter.ParameterType);
				if (parameter.IsOut)
				{
					outputs.Add(repr);
				}
				else
				{
					Contract.Assert(outputs.Count == 0);
					inputs.Add(repr);
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

		public int OutParameterCount
		{
			get { return outputs.Count >= 2 ? outputs.Count : 0; }
		}
		#endregion
	}
}
