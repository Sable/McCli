using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emit = System.Reflection.Emit;

namespace CliKit.Cil
{
	public sealed class FieldReferenceOpcode : Opcode
	{
		#region Constructors
		internal FieldReferenceOpcode(Emit.OpCode opcode) : base(opcode) { }
		#endregion

		#region Properties
		public LocationReferenceKind ReferenceKind
		{
			get
			{
				switch (opcode.StackBehaviourPush)
				{
					case Emit.StackBehaviour.Push1: return LocationReferenceKind.Load;
					case Emit.StackBehaviour.Pushref: return LocationReferenceKind.LoadAddress;
					case Emit.StackBehaviour.Push0: return LocationReferenceKind.Store;
					default: throw new NotImplementedException();
				}
			}
		}

		public bool Static
		{
			get { return opcode.Name[2] == 's'; }
		}
		#endregion
	}
}
