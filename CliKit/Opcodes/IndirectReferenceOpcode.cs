using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emit = System.Reflection.Emit;

namespace CliKit
{
	/// <summary>
	/// Represents an opcode which references the value pointed to by a pointer.
	/// </summary>
	public sealed class IndirectReferenceOpcode : Opcode
	{
		#region Constructors
		internal IndirectReferenceOpcode(Emit.OpCode opcode) : base(opcode) { }
		#endregion

		#region Properties
		public LocationReferenceKind ReferenceKind
		{
			get { return ReflectionEmitEnums.GetLocationReferenceKind(ref opcode); }
		}

		// TODO: add type
		public DataType? DataType
		{
			get { throw new NotImplementedException(); }
		}
		#endregion
	}
}
