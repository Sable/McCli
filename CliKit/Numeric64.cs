using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CliKit
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct Numeric64
	{
		[FieldOffset(0)]
		public long Int64;
		[FieldOffset(0)]
		public ulong UInt64;
		[FieldOffset(0)]
		public double Double;
	}
}
