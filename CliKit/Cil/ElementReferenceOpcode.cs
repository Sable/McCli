using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emit = System.Reflection.Emit;

namespace CliKit.Cil
{
	/// <summary>
	/// Represents opcodes which reference array elements
	/// </summary>
	public sealed class ElementReferenceOpcode : Opcode, ILocationReferenceOpcode
	{
		#region Fields
		private readonly DataType? dataType;
		#endregion

		#region Constructors
		internal ElementReferenceOpcode(Emit.OpCode opcode) : base(opcode)
		{
			string name = opcode.Name;
			int dotIndex = name.LastIndexOf('.');
			if (dotIndex >= 0)
				dataType = DataTypeEnum.TryParseNameInOpcode(name.Substring(dotIndex + 1));
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the way the array elements are referenced.
		/// </summary>
		public LocationReferenceKind ReferenceKind
		{
			get
			{
				// (ld|st)elema?
				var name = Name;
				if (name[0] == 's') return LocationReferenceKind.Store;
				if (name.Length >= 7 && name[6] == 'a') return LocationReferenceKind.LoadAddress;
				return LocationReferenceKind.Load;
			}
		}

		/// <summary>
		/// Gets the CLI data type specified by this opcode, if any.
		/// </summary>
		public DataType? DataType
		{
			get { return dataType; }
		}

		public override PrefixMask ValidPrefixes
		{
			get
			{
				var prefixes = PrefixMask.SuppressCheck;
				if (ReferenceKind == LocationReferenceKind.LoadAddress) prefixes |= PrefixMask.Readonly;
				return prefixes;
			}
		}
		#endregion

		#region Methods
		public override void Accept<T>(OpcodeVisitor<T> visitor, T param)
		{
			visitor.VisitElementReference(this, param);
		}
		#endregion

		#region Explicit Members
		LocationKind ILocationReferenceOpcode.LocationKind
		{
			get { return LocationKind.ArrayElement; }
		}
		#endregion
	}
}
