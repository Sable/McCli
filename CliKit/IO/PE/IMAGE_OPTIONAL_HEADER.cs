using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CliKit.IO.PE
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct IMAGE_OPTIONAL_HEADER32
	{
		public const ushort IMAGE_NT_OPTIONAL_HDR_MAGIC = IMAGE_OPTIONAL_HEADER.IMAGE_NT_OPTIONAL_HDR32_MAGIC;

		public ushort Magic;
		public byte MajorLinkerVersion;
		public byte MinorLinkerVersion;
		public uint SizeOfCode;
		public uint SizeOfInitializedData;
		public uint SizeOfUninitializedData;
		public uint AddressOfEntryPoint;
		public uint BaseOfCode;
		public uint BaseOfData;
		public uint ImageBase;
		public uint SectionAlignment;
		public uint FileAlignment;
		public ushort MajorOperatingSystemVersion;
		public ushort MinorOperatingSystemVersion;
		public ushort MajorImageVersion;
		public ushort MinorImageVersion;
		public ushort MajorSubsystemVersion;
		public ushort MinorSubsystemVersion;
		public uint Win32VersionValue;
		public uint SizeOfImage;
		public uint SizeOfHeaders;
		public uint CheckSum;
		public ushort Subsystem;
		public ushort DllCharacteristics;
		public uint SizeOfStackReserve;
		public uint SizeOfStackCommit;
		public uint SizeOfHeapReserve;
		public uint SizeOfHeapCommit;
		public uint LoaderFlags;
		public uint NumberOfRvaAndSizes;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = IMAGE_OPTIONAL_HEADER.IMAGE_NUMBEROF_DIRECTORY_ENTRIES)]
		public IMAGE_DATA_DIRECTORY[] DataDirectory;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct IMAGE_OPTIONAL_HEADER64
	{
		public const ushort IMAGE_NT_OPTIONAL_HDR_MAGIC = IMAGE_OPTIONAL_HEADER.IMAGE_NT_OPTIONAL_HDR64_MAGIC;

		public ushort Magic;
		public byte MajorLinkerVersion;
		public byte MinorLinkerVersion;
		public uint SizeOfCode;
		public uint SizeOfInitializedData;
		public uint SizeOfUninitializedData;
		public uint AddressOfEntryPoint;
		public uint BaseOfCode;
		public uint BaseOfData;
		public ulong ImageBase;
		public uint SectionAlignment;
		public uint FileAlignment;
		public ushort MajorOperatingSystemVersion;
		public ushort MinorOperatingSystemVersion;
		public ushort MajorImageVersion;
		public ushort MinorImageVersion;
		public ushort MajorSubsystemVersion;
		public ushort MinorSubsystemVersion;
		public uint Win32VersionValue;
		public uint SizeOfImage;
		public uint SizeOfHeaders;
		public uint CheckSum;
		public ushort Subsystem;
		public ushort DllCharacteristics;
		public ulong SizeOfStackReserve;
		public ulong SizeOfStackCommit;
		public ulong SizeOfHeapReserve;
		public ulong SizeOfHeapCommit;
		public uint LoaderFlags;
		public uint NumberOfRvaAndSizes;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = IMAGE_OPTIONAL_HEADER.IMAGE_NUMBEROF_DIRECTORY_ENTRIES)]
		public IMAGE_DATA_DIRECTORY[] DataDirectory;
	}

	public static class IMAGE_OPTIONAL_HEADER
	{
		public const int IMAGE_NUMBEROF_DIRECTORY_ENTRIES = 16;
		public const ushort IMAGE_NT_OPTIONAL_HDR32_MAGIC = 0x010B;
		public const ushort IMAGE_NT_OPTIONAL_HDR64_MAGIC = 0x020B;
		public const ushort IMAGE_ROM_OPTIONAL_HDR_MAGIC = 0x0107;
	}
}
