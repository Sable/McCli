using CliKit.IO.PE;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compiler.CodeGen
{
	/// <summary>
	/// Provides utility methods to help generate Portable Class Libraries.
	/// </summary>
	public static class PortableClassLibrary
	{
		/// <summary>
		/// Adds the assembly attributes required to mark a generated assembly as
		/// being a portable class library.
		/// </summary>
		/// <param name="assemblyBuilder">The assembly builder.</param>
		public static void AddPortableFrameworkAttribute(AssemblyBuilder assemblyBuilder)
		{
			Contract.Requires(assemblyBuilder != null);

			var type = typeof(System.Runtime.Versioning.TargetFrameworkAttribute);
			var frameworkNameConstructor = type.GetConstructor(new[] { typeof(string) });
			var frameworkDisplayNameProperty = type.GetProperty("FrameworkDisplayName");
			var customAttribute = new CustomAttributeBuilder(
				frameworkNameConstructor, new[] { ".NETPortable,Version=v4.0,Profile=Profile136" },
				new[] { frameworkDisplayNameProperty }, new[] { ".NET Portable Subset" });
			assemblyBuilder.SetCustomAttribute(customAttribute);
		}

		/// <summary>
		/// Modifies a CLI assembly produced by <see cref="AssemblyBuilder"/>
		/// to make it a portable class library (huge hack!).
		/// </summary>
		/// <param name="stream">A read-write stream to the CLI assembly to be modified.</param>
		public static void PatchReflectionEmitAssembly(Stream stream)
		{
			Contract.Requires(stream != null);
			Contract.Requires(stream.CanRead && stream.CanWrite && stream.CanSeek);

			// Read the DOS header
			var dosHeader = stream.ReadStruct<IMAGE_DOS_HEADER>();
			if (dosHeader.e_magic != IMAGE_DOS_HEADER.IMAGE_DOS_SIGNATURE)
				throw new InvalidDataException("Invalid DOS header.");

			// Read the NT header
			stream.Position = dosHeader.e_lfanew;
			var ntHeadersSignature = stream.ReadStruct<uint>();
			if (ntHeadersSignature != IMAGE_NT_HEADERS.IMAGE_NT_SIGNATURE)
				throw new InvalidDataException("Invalid NT header.");

			// Read the file header
			var fileHeader = stream.ReadStruct<IMAGE_FILE_HEADER>();
			
			// Read the optional header
			long optionalHeaderPosition = stream.Position;
			var optionalHeaderMagic = stream.ReadStruct<ushort>();
			if (optionalHeaderMagic != IMAGE_OPTIONAL_HEADER32.IMAGE_NT_OPTIONAL_HDR_MAGIC)
				throw new InvalidDataException("Unsupported non-32 bit NT optional header.");

			stream.Position = optionalHeaderPosition;
			var optionalHeader = stream.ReadStruct<IMAGE_OPTIONAL_HEADER32>();

			Contract.Assert(optionalHeader.MajorOperatingSystemVersion == 4);
			Contract.Assert(optionalHeader.NumberOfRvaAndSizes == IMAGE_OPTIONAL_HEADER.IMAGE_NUMBEROF_DIRECTORY_ENTRIES);
			Contract.Assert(optionalHeader.Subsystem == 3);

			// Read the section headers
			var sectionHeaders = new IMAGE_SECTION_HEADER[fileHeader.NumberOfSections];
			for (int i = 0; i < sectionHeaders.Length; ++i)
				sectionHeaders[i] = stream.ReadStruct<IMAGE_SECTION_HEADER>();

			// Read the CLR header
			var comDescriptorVirtualAddress = optionalHeader.DataDirectory[IMAGE_DATA_DIRECTORY.IMAGE_DIRECTORY_ENTRY_COM_DESCRIPTOR].VirtualAddress;
			stream.Position = VirtualAddressToFilePosition(comDescriptorVirtualAddress, sectionHeaders);
			var cor20Header = stream.ReadStruct<IMAGE_COR20_HEADER>();

			// Read the metadata header
			var metadataHeaderVirtualAddress = cor20Header.MetaData.VirtualAddress;
			var metadataHeaderPosition = VirtualAddressToFilePosition(metadataHeaderVirtualAddress, sectionHeaders);
			stream.Position = metadataHeaderPosition;
			var metadataRootPart1 = stream.ReadStruct<MetadataRoot_BeforeVersion>();
			if (metadataRootPart1.Signature != MetadataRoot_BeforeVersion.StandardSignature)
				throw new InvalidDataException("Invalid metadata root signature.");

			string metadataVersionString = stream.ReadNullPaddedString((metadataRootPart1.Length + 3U) & ~3U);
			var metadataRootPart2 = stream.ReadStruct<MetadataRoot_AfterVersion>();

			// Read the stream headers
			var metadataStreamHeaders = new Dictionary<string, StreamHeader>(StringComparer.Ordinal);
			for (int i = 0; i < metadataRootPart2.Streams; ++i)
			{
				var metadataStreamHeader = stream.ReadStruct<StreamHeader>();
				var metadataStreamName = stream.ReadPaddedNullTerminatedString(4);
				metadataStreamHeaders.Add(metadataStreamName, metadataStreamHeader);
			}

			// Read the metadata table stream header
			stream.Position = metadataHeaderPosition + metadataStreamHeaders["#~"].Offset;
			var metadataTablesStream = stream.ReadStruct<MetadataTablesStreamHeader>();
			if (metadataTablesStream.MajorVersion != 2 || metadataTablesStream.MinorVersion != 0)
				throw new InvalidDataException("Unsupported metadata tables stream version.");

			uint[] metadataTableRowCounts = new uint[64];
			for (int i = 0; i < metadataTableRowCounts.Length; ++i)
			{
				if ((metadataTablesStream.Valid & (1UL << i)) != 0)
					metadataTableRowCounts[i] = stream.ReadStruct<uint>();
			}

			// More to be done!
			throw new NotImplementedException("Patching assemblies to make them portable not yet fully implemented.");
		}

		private static TStruct ReadStruct<TStruct>(this Stream stream) where TStruct : struct
		{
			var bytes = new byte[Marshal.SizeOf(typeof(TStruct))];
			stream.ReadAll(bytes, 0, bytes.Length);

			var pin = GCHandle.Alloc(bytes, GCHandleType.Pinned);
			try { return (TStruct)Marshal.PtrToStructure(pin.AddrOfPinnedObject(), typeof(TStruct)); }
			finally { pin.Free(); }
		}

		private static void ReadAll(this Stream stream, byte[] buffer, int offset, int size)
		{
			while (size > 0)
			{
				int read = stream.Read(buffer, offset, size);
				if (read <= 0) throw new EndOfStreamException();

				offset += read;
				size -= read;
			}
		}

		private static long VirtualAddressToFilePosition(uint virtualAddress, IMAGE_SECTION_HEADER[] sectionHeaders)
		{
			for (int i = 0; i < sectionHeaders.Length; ++i)
			{
				long relativeVirtualAddress = (long)virtualAddress - sectionHeaders[i].VirtualAddress;
				if (relativeVirtualAddress >= 0 && relativeVirtualAddress < sectionHeaders[i].SizeOfRawData)
					return sectionHeaders[i].PointerToRawData + relativeVirtualAddress;
			}

			throw new NotImplementedException();
		}

		private static string ReadNullPaddedString(this Stream stream, uint length)
		{
			byte[] bytes = new byte[length];
			stream.ReadAll(bytes, 0, bytes.Length);

			int stringLength = Array.IndexOf<byte>(bytes, (byte)0);
			if (stringLength == -1) stringLength = bytes.Length;

			return Encoding.UTF8.GetString(bytes, 0, stringLength);
		}

		private static string ReadPaddedNullTerminatedString(this Stream stream, int alignment)
		{
			var bytes = new List<byte>();
			while (true)
			{
				var @byte = stream.ReadByte();
				if (unchecked((byte)@byte) != @byte) throw new EndOfStreamException();
				if (@byte == 0) break;

				bytes.Add(unchecked((byte)@byte));
			}

			int paddingLength = alignment - (bytes.Count + 1) % alignment;
			if (paddingLength < alignment)
			{
				while (paddingLength > 0)
				{
					stream.ReadByte();
					paddingLength--;
				}
			}

			return Encoding.UTF8.GetString(bytes.ToArray());
		}
	}
}
