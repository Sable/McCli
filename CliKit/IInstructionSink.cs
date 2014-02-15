using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliKit
{
	/// <summary>
	/// Interface for objects which consume CIL instructions.
	/// </summary>
	public interface IInstructionSink
	{
		void Write(ref Instruction instruction);
	}

	// TODO: Lots of extension methods
	public static class InstructionSink
	{
		public static void Write(this IInstructionSink sink, Opcode opcode, Operand operand)
		{
			var instruction = new Instruction(opcode, operand);
			sink.Write(ref instruction);
		}

		public static void WriteLoad(this IInstructionSink sink, VariableLocation location)
		{
			var opcode = Opcode.Load(location);

		}
	}
}
