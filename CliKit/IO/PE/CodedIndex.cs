using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table = CliKit.MetadataTokenKind;

namespace CliKit.IO.PE
{
	/// <summary>
	/// Represents the various forms of coded index encodings for metadata table references,
	/// as defined in ECMA 335 II.24.2.6
	/// </summary>
	public sealed class CodedIndex
	{
		#region Fields
		private const byte unused = 0xFF;

		public static readonly CodedIndex TypeDefOrRef = new CodedIndex(
			(byte)Table.Type, (byte)Table.TypeReference, (byte)Table.TypeSpecification);
		public static readonly CodedIndex HasConstant = new CodedIndex(
			(byte)Table.Field, (byte)Table.Parameter, (byte)Table.Property);
		public static readonly CodedIndex HasCustomAttribute = new CodedIndex(
			(byte)Table.Method, (byte)Table.Field, (byte)Table.TypeReference, (byte)Table.Type,
			(byte)Table.Parameter, (byte)Table.InterfaceImplementation, (byte)Table.MemberReference, (byte)Table.Module,
			(byte)Table.DeclarativeSecurity, (byte)Table.Property, (byte)Table.Event, (byte)Table.Signature,
			(byte)Table.ModuleReference, (byte)Table.TypeSpecification, (byte)Table.Assembly, (byte)Table.AssemblyReference,
			(byte)Table.Field, (byte)Table.ExportedType, (byte)Table.ManifestResource);
		public static readonly CodedIndex HasFieldMarshal = new CodedIndex(
			(byte)Table.Field, (byte)Table.Parameter);
		public static readonly CodedIndex HasDeclSecurity = new CodedIndex(
			(byte)Table.Type, (byte)Table.Method, (byte)Table.Assembly);
		public static readonly CodedIndex MemberRefParent = new CodedIndex(
			(byte)Table.Type, (byte)Table.TypeReference, (byte)Table.ModuleReference, (byte)Table.Method, (byte)Table.TypeSpecification);
		public static readonly CodedIndex HasSemantics = new CodedIndex(
			(byte)Table.Event, (byte)Table.Property);
		public static readonly CodedIndex MethodDefOrRef = new CodedIndex(
			(byte)Table.Method, (byte)Table.MemberReference);
		public static readonly CodedIndex MemberForwarded = new CodedIndex(
			(byte)Table.Field, (byte)Table.Method);
		public static readonly CodedIndex Implementation = new CodedIndex
			((byte)Table.Field, (byte)Table.AssemblyReference, (byte)Table.ExportedType);
		public static readonly CodedIndex CustomAttributeType = new CodedIndex(
			unused, unused, (byte)Table.Method, (byte)Table.MemberReference, unused);
		public static readonly CodedIndex ResolutionScope = new CodedIndex(
			(byte)Table.Module, (byte)Table.ModuleReference, (byte)Table.AssemblyReference, (byte)Table.TypeReference);
		public static readonly CodedIndex TypeOrMethodDef = new CodedIndex(
			(byte)Table.Type, (byte)Table.Method);

		private readonly byte[] tables;
		private readonly int bitCount;
		#endregion

		#region Constructors
		internal CodedIndex(params byte[] tables)
		{
			this.tables = tables;
			this.bitCount = (int)Math.Ceiling(Math.Log(tables.Length, 2));
		}
		#endregion

		#region Properties
		public int TableCount
		{
			get { return tables.Length; }
		}

		public int TagBitCount
		{
			get { return bitCount; }
		}
		#endregion

		#region Methods
		public Table GetTable(int tag)
		{
			return (Table)tables[tag];
		}
		#endregion
	}
}
