using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CliKit.IO
{
	public struct MethodBodyVerificationContext
	{
		private int maxStackSize;

		public MethodBase Method;
		public Type[] ParameterTypes;
		public Type ReturnType;
		public bool HasInitLocals;

		public int MaxStackSize
		{
			get { return maxStackSize; }
			set
			{
				Contract.Requires(value >= 0 && value <= ushort.MaxValue);
				maxStackSize = value;
			}
		}
	}
}
