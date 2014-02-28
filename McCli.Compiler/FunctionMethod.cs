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
		private readonly FunctionSignature signature;
		#endregion

		#region Constructors
		public FunctionMethod(MethodInfo method)
		{
			Contract.Requires(method != null);

			this.method = method;
			this.signature = new FunctionSignature(method.GetParameters(), method.ReturnType);
		}

		public FunctionMethod(MethodInfo method, FunctionSignature signature)
		{
			Contract.Requires(method != null);
			Contract.Requires(signature != null);

			this.method = method;
			this.signature = signature;
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

		public FunctionSignature Signature
		{
			get { return signature; }
		}
		#endregion

		#region Methods
		public override string ToString()
		{
			return string.Format("{0} : {1}", method.Name, signature);
		}
		#endregion
	}
}
