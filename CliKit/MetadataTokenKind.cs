using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliKit
{
	/// <summary>
	/// Identifies a kind of metadata token.
	/// Values match the most significant byte of metadata tokens.
	/// </summary>
	public enum MetadataTokenKind
	{
		Module = 0x00,
		TypeReference = 0x01,
		Type = 0x02,
		Field = 0x04,
		Method = 0x06,
		Parameter = 0x08,
		InterfaceImplementation = 0x09,
		MemberReference = 0x0A,
		Constant = 0x0B,
		CustomAttribute = 0x0C,
		FieldMarshal = 0x0D,
		DeclarativeSecurity = 0x0E,
		ClassLayout = 0x0F,
		FieldLayout = 0x10,
		Signature = 0x11,
		EventMap = 0x12,
		Event = 0x14,
		PropertyMap = 0x15,
		Property = 0x17,
		MethodSemantics = 0x18,
		MethodImplementation = 0x19,
		ModuleReference = 0x1A,
		TypeSpecification = 0x1B,
		ImplementationMap = 0x1C,
		FieldRelativeVirtualAddress = 0x1D,
		Assembly = 0x20,
		AssemblyProcessor = 0x21,
		AssemblyOS = 0x22,
		AssemblyReference = 0x23,
		AssemblyReferenceProcessor = 0x24,
		AssemblyReferenceOS = 0x25,
		File = 0x26,
		ExportedType = 0x27,
		ManifestResource = 0x28,
		NestedClass = 0x29,
		GenericParameter = 0x2A,
		MethodSpecification = 0x2B,
		GenericParameterConstraint = 0x2C,
		String = 0x70,
		Name = 0x71,
		BaseType = 0x72,
	}
}
