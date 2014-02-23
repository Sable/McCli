using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CliKit.IO.PE
{
	/// <summary>
	/// Represents the "#~" stream, as defined in ECMA 335 II.24.2.6.
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct MetadataTablesStreamHeader
	{
		public uint Reserved0; // Always 0
		public byte MajorVersion;
		public byte MinorVersion;
		public byte HeapSizes;
		public byte Reserved1; // Always 1
		public ulong Valid;
		public ulong Sorted;
		// Row counts per table follow
		// Metadata tables follow
	}
}
