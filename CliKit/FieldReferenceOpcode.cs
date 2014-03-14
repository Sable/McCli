using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emit = System.Reflection.Emit;

namespace CliKit
{
	/// <summary>
	/// Represents opcode which reference a field.
	/// </summary>
	public sealed class FieldReferenceOpcode : Opcode, ILocationReferenceOpcode
	{
		#region Constructors
		internal FieldReferenceOpcode(Emit.OpCode opcode) : base(opcode) { }
		#endregion

		#region Properties
		public LocationReferenceKind ReferenceKind
		{
			get
			{
				var name = Name;
				if (name[0] == 's') return LocationReferenceKind.Store;
				if (name[name.Length - 1] == 'a') return LocationReferenceKind.LoadAddress;
				return LocationReferenceKind.Load;
			}
		}

		public bool IsStatic
		{
			get
			{
				var name = Name;
				return name[2] == 's';
			}
		}

		public override PrefixMask ValidPrefixes
		{
			get
			{
				var prefixes = PrefixMask.None;
				if (ReferenceKind == LocationReferenceKind.LoadAddress) prefixes |= PrefixMask.Volatile;
				if (!IsStatic) prefixes |= PrefixMask.Unaligned | PrefixMask.SuppressCheck;
				return prefixes;
			}
		}
		#endregion

		#region Methods
		public override void Accept<T>(OpcodeVisitor<T> visitor, T param)
		{
			visitor.VisitFieldReference(this, param);
		}
		#endregion

		#region Explicit Members
		LocationKind ILocationReferenceOpcode.LocationKind
		{
			get { return LocationKind.Field; }
		}
		#endregion
	}
}
