using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emit = System.Reflection.Emit;

namespace CliKit
{
	/// <summary>
	/// An opcode that refers to a variable
	/// </summary>
	public sealed class VariableReferenceOpcode : Opcode
	{
		#region Constructors
		internal VariableReferenceOpcode(Emit.OpCode opcode) : base(opcode) {}
		#endregion

		#region Properties
		public VariableKind LocalKind
		{
			get { return opcode.Name[2] == 'a' ? VariableKind.Parameter : VariableKind.Local; }
		}

		public LocationReferenceKind ReferenceKind
		{
			get { return ReflectionEmitEnums.GetLocationReferenceKind(ref opcode); }
		}

		public IntegerOperandForm OperandForm
		{
			get
			{
				char lastChar = opcode.Name[opcode.Name.Length - 1];
				if (lastChar >= '0' && lastChar <= '9') return IntegerOperandForm.Constant;
				if (lastChar == 'S') return IntegerOperandForm.Short;
				return IntegerOperandForm.Normal;
			}
		}

		public int? ConstantIndex
		{
			get
			{
				char lastChar = opcode.Name[opcode.Name.Length - 1];
				return lastChar >= '0' && lastChar < '9' ? (lastChar - '0') : (int?)null;
			}
		}
		#endregion

		#region Methods
		#endregion
	}
}
