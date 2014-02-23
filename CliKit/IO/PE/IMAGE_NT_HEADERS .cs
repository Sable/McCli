using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CliKit.IO.PE
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct IMAGE_NT_HEADERS32
	{
		public uint Signature;
		public IMAGE_FILE_HEADER FileHeader;
		public IMAGE_OPTIONAL_HEADER32 OptionalHeader;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct IMAGE_NT_HEADERS64
	{
		public uint Signature;
		public IMAGE_FILE_HEADER FileHeader;
		public IMAGE_OPTIONAL_HEADER64 OptionalHeader;
	}

	public static class IMAGE_NT_HEADERS
	{
		public const uint IMAGE_NT_SIGNATURE = 0x00004550;
	}
}
