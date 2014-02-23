using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CliKit.IO.PE
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct MetadataRoot_BeforeVersion
	{
		public const uint StandardSignature = 0x424A5342;

		public uint Signature;
		public ushort MajorVersion;
		public ushort MinorVersion;
		public uint Reserved;
		public uint Length;
		// Null-padded version string of the given length follows
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct MetadataRoot_AfterVersion
	{
		public ushort Flags;
		public ushort Streams;
		// Stream header array follows
	}
}
