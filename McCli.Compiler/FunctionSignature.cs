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
		private readonly List<MRepr> inputReprs = new List<MRepr>();
		private readonly int variadicInputIndex;
		private readonly List<MRepr> outputReprs = new List<MRepr>();
		private readonly bool hasVariadicOutputs;
		#endregion

		#region Constructors
		public FunctionSignature(ParameterInfo[] parameters, Type returnType)
		{
			Contract.Requires(parameters != null);
			Contract.Requires(returnType != null);

			variadicInputIndex = -1;

			foreach (var parameter in parameters)
			{
				bool variadic;
				MRepr repr;
				if (parameter.ParameterType.IsArray)
				{
					variadic = true;
					repr = MRepr.FromCliType(parameter.ParameterType.GetElementType());
				}
				else
				{
					variadic = false;
					repr = MRepr.FromCliType(parameter.ParameterType);
				}

				if (parameter.IsOut)
				{
					Contract.Assert(!hasVariadicOutputs);
					if (variadic) hasVariadicOutputs = true;
					outputReprs.Add(repr);
				}
				else
				{
					Contract.Assert(outputReprs.Count == 0);
					if (variadic)
					{
						Contract.Assert(variadicInputIndex < 0);
						variadicInputIndex = inputReprs.Count;
					}

					inputReprs.Add(repr);
				}
			}
		}
		#endregion

		#region Properties
		public bool HasVariadicInputs
		{
			get { return variadicInputIndex >= 0; }
		}

		public int? InputCount
		{
			get { return HasVariadicInputs ? (int?)null : inputReprs.Count; }
		}

		public int FixedInputCount
		{
			get { return HasVariadicInputs ? inputReprs.Count - 1 : inputReprs.Count; }
		}

		public int? VariadicInputIndex
		{
			get { return variadicInputIndex == -1 ? (int?)null : variadicInputIndex; }
		}

		public bool HasVariadicOutputs
		{
			get { return hasVariadicOutputs; }
		}

		public int? OutputCount
		{
			get { return hasVariadicOutputs ? (int?)null : outputReprs.Count; }
		}
		#endregion

		#region Methods
		public MRepr GetFixedInputRepr(int index)
		{
			if (HasVariadicInputs && index >= variadicInputIndex) index++;
			return inputReprs[index];
		}

		public MRepr GetInputRepr(int index, int count)
		{
			Contract.Requires(HasVariadicInputs ? (count >= FixedInputCount) : (count == FixedInputCount));
			throw new NotImplementedException();
		}

		public MRepr GetVariadicInputRepr()
		{
			if (!HasVariadicInputs) throw new InvalidOperationException();
			return inputReprs[variadicInputIndex];
		}

		public MRepr GetOutputRepr(int index)
		{
			return hasVariadicOutputs ? outputReprs[0] : outputReprs[index];
		}
		#endregion
	}
}
