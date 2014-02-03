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
		private readonly ImmutableArray<MClass>? parameterClasses;
		private readonly ImmutableArray<MClass>? returnValueClasses;
		#endregion

		#region Constructors
		public FunctionInfo(MethodInfo method)
		{
			Contract.Requires(method.IsStatic && method.IsPublic && !method.IsGenericMethodDefinition);

			this.method = method;
		}
		#endregion

		#region Properties
		#endregion

		#region Methods
		#endregion
	}
}
