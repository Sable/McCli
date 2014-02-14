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
}
