using CliKit;
using CliKit.IO.PE;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Table = CliKit.MetadataTokenKind;

namespace McCli.Compiler.CodeGen
{
	/// <summary>
	/// Provides utility methods to help generate Portable Class Libraries.
	/// </summary>
	public static class PortableClassLibrary
	{
		private struct MetadataTableInfo
		{
			public uint Offset;
			public uint RowCount;
			public uint BytesPerRow;
		}

		private static readonly byte[] nonportableMscorlibPublicKeyBlob = new byte[] { 0x08, 0xB7, 0x7A, 0x5C, 0x56, 0x19, 0x34, 0xE0, 0x89 };
		private static readonly Version nonportableMscorlibVersion = new Version(4, 0, 0, 0);
		private static readonly uint nonportableMscorlibFlags = (uint)AssemblyNameFlags.None;
		private static readonly byte[] portableMscorlibPublicKeyBlob = new byte[] { 0x08, 0x7C, 0xEC, 0x85, 0xD7, 0xBE, 0xA7, 0x79, 0x8E };
		private static readonly Version portableMscorlibVersion = new Version(2, 0, 5, 0);
		private static readonly uint portableMscorlibFlags = (uint)AssemblyNameFlags.Retargetable;

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
			var metadataTablesStreamHeader = stream.ReadStruct<MetadataTablesStreamHeader>();
			if (metadataTablesStreamHeader.MajorVersion != 2 || metadataTablesStreamHeader.MinorVersion != 0)
				throw new InvalidDataException("Unsupported metadata tables stream version.");

			var metadataTableInfos = new MetadataTableInfo[64];
			for (int i = 0; i < metadataTableInfos.Length; ++i)
			{
				if ((metadataTablesStreamHeader.Valid & (1UL << i)) != 0)
					metadataTableInfos[i].RowCount = stream.ReadStruct<uint>();
			}

			var metadataTablesStartPosition = stream.Position;

			// Compute the size of the metadata rows and their offset
			uint stringIndexSize = (metadataTablesStreamHeader.HeapSizes & 0x01) == 0 ? 2U : 4U;
			uint blobIndexSize = (metadataTablesStreamHeader.HeapSizes & 0x04) == 0 ? 2U : 4U;
			FillMetadataTableInfos(ref metadataTablesStreamHeader, metadataTableInfos);

			// Read the assembly reference tables
			var assemblyReferenceTableInfo = metadataTableInfos[(int)Table.AssemblyReference];
			stream.Position = metadataTablesStartPosition + assemblyReferenceTableInfo.Offset;
			for (uint i = 0; i < assemblyReferenceTableInfo.RowCount; ++i)
			{
				var rowPosition = stream.Position;
				ushort majorVersion = stream.ReadStruct<ushort>();
				ushort minorVersion = stream.ReadStruct<ushort>();
				ushort buildNumber = stream.ReadStruct<ushort>();
				ushort revisionNumber = stream.ReadStruct<ushort>();
				var version = new Version(majorVersion, minorVersion, buildNumber, revisionNumber);
				uint flags = stream.ReadStruct<uint>();
				uint publicKeyOrTokenIndex = blobIndexSize == 2 ? stream.ReadStruct<ushort>() : stream.ReadStruct<uint>();
				uint nameIndex = stringIndexSize == 2 ? stream.ReadStruct<ushort>() : stream.ReadStruct<uint>();
				uint cultureIndex = stringIndexSize == 2 ? stream.ReadStruct<ushort>() : stream.ReadStruct<uint>();
				uint hashValueIndex = stringIndexSize == 2 ? stream.ReadStruct<ushort>() : stream.ReadStruct<uint>();

				var nextRowPosition = stream.Position;
				if (version == nonportableMscorlibVersion && flags == nonportableMscorlibFlags)
				{
					// Looks like mscorlib, let's make sure
					long nameStringPosition = metadataHeaderPosition + metadataStreamHeaders["#Strings"].Offset + nameIndex;
					long publicKeyBlobPosition = metadataHeaderPosition + metadataStreamHeaders["#Blob"].Offset + publicKeyOrTokenIndex;

					stream.Position = nameStringPosition;
					var name = stream.ReadPaddedNullTerminatedString(1);
					stream.Position = publicKeyBlobPosition;
					var publicKeyBlob = stream.ReadBytes(nonportableMscorlibPublicKeyBlob.Length);
					if (name == "mscorlib" && Equals(publicKeyBlob, nonportableMscorlibPublicKeyBlob))
					{
						// It is indeed the mscorlib we want to patch!
						// Patch the version number and assembly flags
						stream.Position = rowPosition;
						stream.WriteStruct((ushort)portableMscorlibVersion.Major);
						stream.WriteStruct((ushort)portableMscorlibVersion.Minor);
						stream.WriteStruct((ushort)portableMscorlibVersion.Build);
						stream.WriteStruct((ushort)portableMscorlibVersion.Revision);
						stream.WriteStruct<uint>(portableMscorlibFlags);

						// Update the public key token
						stream.Position = publicKeyBlobPosition;
						stream.Write(portableMscorlibPublicKeyBlob, 0, portableMscorlibPublicKeyBlob.Length);

						// Done!
						return;
					}
					else
					{
						stream.Position = nextRowPosition;
					}
				}
			}

			throw new InvalidDataException("mscorlib assembly reference not found.");
		}

		private static void FillMetadataTableInfos(
			ref MetadataTablesStreamHeader header, MetadataTableInfo[] tables)
		{
			// Compute the size of the metadata tables
			uint stringIndexSize = (header.HeapSizes & 0x01) == 0 ? 2U : 4U;
			uint guidIndexSize = (header.HeapSizes & 0x02) == 0 ? 2U : 4U;
			uint blobIndexSize = (header.HeapSizes & 0x04) == 0 ? 2U : 4U;

			tables[(int)Table.Module].BytesPerRow = 2 + stringIndexSize + guidIndexSize * 3;
			tables[(int)Table.TypeReference].BytesPerRow = GetIndexSize(CodedIndex.ResolutionScope, tables) + stringIndexSize * 2;
			tables[(int)Table.Type].BytesPerRow = 4 + stringIndexSize * 2 + GetIndexSize(CodedIndex.TypeDefOrRef, tables)
				+ GetIndexSize(Table.Field, tables) + GetIndexSize(Table.Method, tables);
			tables[(int)Table.Field].BytesPerRow = 2 + stringIndexSize + blobIndexSize;
			tables[(int)Table.Method].BytesPerRow = 8 + stringIndexSize + blobIndexSize + GetIndexSize(Table.Parameter, tables);
			tables[(int)Table.Parameter].BytesPerRow = 4 + stringIndexSize;
			tables[(int)Table.InterfaceImplementation].BytesPerRow = GetIndexSize(Table.Type, tables) + GetIndexSize(CodedIndex.TypeDefOrRef, tables);
			tables[(int)Table.MemberReference].BytesPerRow = GetIndexSize(CodedIndex.MemberRefParent, tables) + stringIndexSize + blobIndexSize;
			tables[(int)Table.Constant].BytesPerRow = 2 + GetIndexSize(CodedIndex.HasConstant, tables) + blobIndexSize;
			tables[(int)Table.CustomAttribute].BytesPerRow = GetIndexSize(CodedIndex.HasCustomAttribute, tables)
				+ GetIndexSize(CodedIndex.CustomAttributeType, tables) + blobIndexSize;
			tables[(int)Table.FieldMarshal].BytesPerRow = GetIndexSize(CodedIndex.HasFieldMarshal, tables) + blobIndexSize;
			tables[(int)Table.DeclarativeSecurity].BytesPerRow = 2 + GetIndexSize(CodedIndex.HasDeclSecurity, tables) + blobIndexSize;
			tables[(int)Table.ClassLayout].BytesPerRow = 6 + GetIndexSize(Table.Type, tables);
			tables[(int)Table.FieldLayout].BytesPerRow = 4 + GetIndexSize(Table.Field, tables);
			tables[(int)Table.Signature].BytesPerRow = blobIndexSize;
			tables[(int)Table.EventMap].BytesPerRow = GetIndexSize(Table.Type, tables) + GetIndexSize(Table.Event, tables);
			tables[(int)Table.Event].BytesPerRow = 2 + stringIndexSize + GetIndexSize(CodedIndex.TypeDefOrRef, tables);
			tables[(int)Table.PropertyMap].BytesPerRow = GetIndexSize(Table.Type, tables) + GetIndexSize(Table.Property, tables);
			tables[(int)Table.Property].BytesPerRow = 2 + stringIndexSize + blobIndexSize;
			tables[(int)Table.MethodSemantics].BytesPerRow = 2 + GetIndexSize(Table.Method, tables) + GetIndexSize(CodedIndex.HasSemantics, tables);
			tables[(int)Table.MethodImplementation].BytesPerRow = GetIndexSize(Table.Type, tables) + GetIndexSize(CodedIndex.MethodDefOrRef, tables) * 2;
			tables[(int)Table.ModuleReference].BytesPerRow = stringIndexSize;
			tables[(int)Table.TypeSpecification].BytesPerRow = blobIndexSize;
			tables[(int)Table.ImplementationMap].BytesPerRow = 2 + GetIndexSize(CodedIndex.MemberForwarded, tables)
				+ stringIndexSize + GetIndexSize(Table.ModuleReference, tables);
			tables[(int)Table.FieldRva].BytesPerRow = 4 + GetIndexSize(Table.Field, tables);
			tables[(int)Table.Assembly].BytesPerRow = 16 + blobIndexSize + stringIndexSize * 2;
			tables[(int)Table.AssemblyProcessor].BytesPerRow = 4;
			tables[(int)Table.AssemblyOS].BytesPerRow = 12;
			tables[(int)Table.AssemblyReference].BytesPerRow = 12 + blobIndexSize * 2 + stringIndexSize * 2;
			tables[(int)Table.AssemblyReferenceProcessor].BytesPerRow = 4 + GetIndexSize(Table.AssemblyReference, tables);
			tables[(int)Table.AssemblyReferenceOS].BytesPerRow = 12 + GetIndexSize(Table.AssemblyReference, tables);
			tables[(int)Table.File].BytesPerRow = 4 + stringIndexSize + blobIndexSize;
			tables[(int)Table.ExportedType].BytesPerRow = 8 + stringIndexSize * 2 + GetIndexSize(CodedIndex.Implementation, tables);
			tables[(int)Table.ManifestResource].BytesPerRow = 8 + stringIndexSize + GetIndexSize(CodedIndex.Implementation, tables);
			tables[(int)Table.NestedClass].BytesPerRow = GetIndexSize(Table.NestedClass, tables) * 2;
			tables[(int)Table.GenericParameter].BytesPerRow = 4 + GetIndexSize(CodedIndex.TypeOrMethodDef, tables) + stringIndexSize;
			tables[(int)Table.MethodSpecification].BytesPerRow = GetIndexSize(CodedIndex.MethodDefOrRef, tables) + blobIndexSize;
			tables[(int)Table.GenericParameterConstraint].BytesPerRow = GetIndexSize(Table.GenericParameter, tables) + GetIndexSize(CodedIndex.TypeDefOrRef, tables);

			uint offset = 0;
			for (int i = 0; i < tables.Length; ++i)
			{
				if (tables[i].RowCount > 0 && tables[i].BytesPerRow == 0)
					throw new NotImplementedException();

				tables[i].Offset = offset;
				offset += tables[i].BytesPerRow * tables[i].RowCount;
			}
		}

		private static uint GetIndexSize(CodedIndex codedIndex, MetadataTableInfo[] tables)
		{
			uint maxRowCount = 0;
			for (int i = 0; i < codedIndex.TableCount; ++i)
			{
				var table = codedIndex.GetTable(i);
				if (table.HasValue)
				{
					var rowCount = tables[(int)table.Value].RowCount;
					if (rowCount > maxRowCount) maxRowCount = rowCount;
				}
			}

			int valueBitCount = 16 - codedIndex.TagBitCount;
			return maxRowCount < (1U << valueBitCount) ? 2U : 4U;
		}

		private static uint GetIndexSize(Table table, MetadataTableInfo[] tables)
		{
			return tables[(int)table].RowCount <= ushort.MaxValue ? 2U : 4U;
		}

		private static long VirtualAddressToFilePosition(uint virtualAddress, IMAGE_SECTION_HEADER[] sectionHeaders)
		{
			for (int i = 0; i < sectionHeaders.Length; ++i)
			{
				long relativeVirtualAddress = (long)virtualAddress - sectionHeaders[i].VirtualAddress;
				if (relativeVirtualAddress >= 0 && relativeVirtualAddress < sectionHeaders[i].SizeOfRawData)
					return sectionHeaders[i].PointerToRawData + relativeVirtualAddress;
			}

			throw new InvalidDataException("Could not resolve virtual address.");
		}

		private static TStruct ReadStruct<TStruct>(this Stream stream) where TStruct : struct
		{
			var bytes = new byte[Marshal.SizeOf(typeof(TStruct))];
			stream.ReadAll(bytes, 0, bytes.Length);

			var pin = GCHandle.Alloc(bytes, GCHandleType.Pinned);
			try { return (TStruct)Marshal.PtrToStructure(pin.AddrOfPinnedObject(), typeof(TStruct)); }
			finally { pin.Free(); }
		}

		private static void WriteStruct<TStruct>(this Stream stream, TStruct value) where TStruct : struct
		{
			var bytes = new byte[Marshal.SizeOf(typeof(TStruct))];
			var pin = GCHandle.Alloc(bytes, GCHandleType.Pinned);
			try { Marshal.StructureToPtr(value, pin.AddrOfPinnedObject(), fDeleteOld: false); }
			finally { pin.Free(); }
			stream.Write(bytes, 0, bytes.Length);
		}

		private static byte[] ReadBytes(this Stream stream, int size)
		{
			var bytes = new byte[size];
			ReadAll(stream, bytes, 0, size);
			return bytes;
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

		private static bool Equals(byte[] first, byte[] second)
		{
			if (first.Length != second.Length) return false;
			for (int i = 0; i < first.Length; ++i)
				if (first[i] != second[i])
					return false;
			return true;
		}
	}
}
