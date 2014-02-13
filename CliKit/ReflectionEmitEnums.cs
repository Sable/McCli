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
		public static LocationReferenceKind GetLocationReferenceKind(ref Emit.OpCode opcode)
		{
			return GetLocationReferenceKindFromStackBehaviourPush(opcode.StackBehaviourPush);
		}

		public static LocationReferenceKind GetLocationReferenceKindFromStackBehaviourPush(Emit.StackBehaviour stackBehaviour)
		{
			switch (stackBehaviour)
			{
				case Emit.StackBehaviour.Push1:
				case Emit.StackBehaviour.Pushi:
				case Emit.StackBehaviour.Pushi8:
				case Emit.StackBehaviour.Pushr4:
				case Emit.StackBehaviour.Pushr8:
					return LocationReferenceKind.Load;

				case Emit.StackBehaviour.Pushref:
					return LocationReferenceKind.LoadAddress;

				case Emit.StackBehaviour.Push0:
					return LocationReferenceKind.Store;

				default:
					throw new ArgumentException("stackBehaviour");
			}
		}
	}
}
