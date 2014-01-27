using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CliKit
{
	public enum StackEntryKind
	{
		Int32,
		Int64,
		NativeInt,
		FloatingPoint,
		ManagedPointer,
		ControlledMutabilityManagedPointer,
		UnmanagedPointer,
		ObjectReference,
		ValueType
	}
}
