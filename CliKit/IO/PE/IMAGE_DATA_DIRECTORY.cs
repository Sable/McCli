using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CliKit.IO.PE
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct IMAGE_DATA_DIRECTORY
	{
		public const int IMAGE_DIRECTORY_ENTRY_EXPORT = 0;
		public const int IMAGE_DIRECTORY_ENTRY_IMPORT = 1;
		public const int IMAGE_DIRECTORY_ENTRY_RESOURCE = 2;
		public const int IMAGE_DIRECTORY_ENTRY_EXCEPTION = 3;
		public const int IMAGE_DIRECTORY_ENTRY_SECURITY = 4;
		public const int IMAGE_DIRECTORY_ENTRY_BASERELOC = 5;
		public const int IMAGE_DIRECTORY_ENTRY_DEBUG = 6;
		public const int IMAGE_DIRECTORY_ENTRY_COPYRIGHT = 7;
		public const int IMAGE_DIRECTORY_ENTRY_GLOBALPTR = 8;
		public const int IMAGE_DIRECTORY_ENTRY_TLS = 9;
		public const int IMAGE_DIRECTORY_ENTRY_LOAD_CONFIG = 10;
		public const int IMAGE_DIRECTORY_ENTRY_BOUND_IMPORT = 11;
		public const int IMAGE_DIRECTORY_ENTRY_IAT = 12;
		public const int IMAGE_DIRECTORY_ENTRY_DELAY_IMPORT = 13;
		public const int IMAGE_DIRECTORY_ENTRY_COM_DESCRIPTOR = 14;

		public uint VirtualAddress;
		public uint Size;
	}
}
