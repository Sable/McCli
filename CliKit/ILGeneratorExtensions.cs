using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CliKit
{
	public static class ILGeneratorExtensions
	{
		public static void EmitLoad(this ILGenerator generator, VariableLocation location)
		{
			Contract.Requires(generator != null);

			if (location.IsLocal)
				generator.Emit(OpCodes.Ldloc_S, checked((byte)location.Index));
			else
				generator.Emit(OpCodes.Ldarg_S, checked((byte)location.Index));
		}

		public static void EmitLoadAddress(this ILGenerator generator, VariableLocation location)
		{
			Contract.Requires(generator != null);

			if (location.IsLocal)
				generator.Emit(OpCodes.Ldloca_S, checked((byte)location.Index));
			else
				generator.Emit(OpCodes.Ldarga_S, checked((byte)location.Index));
		}

		public static void EmitStore(this ILGenerator generator, VariableLocation location)
		{
			Contract.Requires(generator != null);

			if (location.IsLocal)
				generator.Emit(OpCodes.Stloc_S, checked((byte)location.Index));
			else
				generator.Emit(OpCodes.Starg_S, checked((byte)location.Index));
		}
	}
}
