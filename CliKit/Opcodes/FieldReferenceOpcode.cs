using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emit = System.Reflection.Emit;

namespace CliKit
{
	/// <summary>
	/// Represents opcode which reference a field (load, load address, store).
	/// </summary>
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

		public bool IsStatic
		{
			get { return opcode.StackBehaviourPop == Emit.StackBehaviour.Pop0; }
		}
		#endregion
	}
}
