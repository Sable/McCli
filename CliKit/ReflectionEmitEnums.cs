using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emit = System.Reflection.Emit;

namespace CliKit
{
	/// <summary>
	/// Providies utility methods to convert values from enumerations in the <see cref="N:System.Reflection.Emit"/> namespace.
	/// </summary>
	internal static class ReflectionEmitEnums
	{
		public static int? GetStackDelta(Emit.StackBehaviour stackBehaviour)
		{
			switch (stackBehaviour)
			{
				case Emit.StackBehaviour.Popi_popi_popi:
				case Emit.StackBehaviour.Popref_popi_pop1:
				case Emit.StackBehaviour.Popref_popi_popi:
				case Emit.StackBehaviour.Popref_popi_popi8:
				case Emit.StackBehaviour.Popref_popi_popr4:
				case Emit.StackBehaviour.Popref_popi_popr8:
				case Emit.StackBehaviour.Popref_popi_popref:
					return -3;

				case Emit.StackBehaviour.Pop1_pop1:
				case Emit.StackBehaviour.Popi_pop1:
				case Emit.StackBehaviour.Popi_popi:
				case Emit.StackBehaviour.Popi_popi8:
				case Emit.StackBehaviour.Popi_popr4:
				case Emit.StackBehaviour.Popi_popr8:
				case Emit.StackBehaviour.Popref_pop1:
				case Emit.StackBehaviour.Popref_popi:
					return -2;

				case Emit.StackBehaviour.Pop1:
				case Emit.StackBehaviour.Popi:
				case Emit.StackBehaviour.Popref:
					return -1;

				case Emit.StackBehaviour.Pop0:
				case Emit.StackBehaviour.Push0:
					return 0;

				case Emit.StackBehaviour.Push1:
				case Emit.StackBehaviour.Pushi:
				case Emit.StackBehaviour.Pushi8:
				case Emit.StackBehaviour.Pushr4:
				case Emit.StackBehaviour.Pushr8:
				case Emit.StackBehaviour.Pushref:
					return 1;

				case Emit.StackBehaviour.Push1_push1:
					return 2;

				case Emit.StackBehaviour.Varpop:
				case Emit.StackBehaviour.Varpush:
					return null;

				default:
					throw new ArgumentException("stackBehaviour");
			}
		}
	}
}
