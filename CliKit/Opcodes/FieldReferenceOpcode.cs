using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emit = System.Reflection.Emit;

namespace CliKit
{
	public sealed class FieldReferenceOpcode : Opcode
	{
		#region Constructors
		internal FieldReferenceOpcode(Emit.OpCode opcode) : base(opcode) { }
		#endregion

		#region Properties
		public LocationReferenceKind ReferenceKind
		{
			get { return ReflectionEmitEnums.GetLocationReferenceKind(ref opcode); }
		}

		public bool Static
		{
			get { return opcode.Name[2] == 's'; }
		}
		#endregion
	}
}
