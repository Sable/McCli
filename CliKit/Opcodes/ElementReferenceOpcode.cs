using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emit = System.Reflection.Emit;

namespace CliKit
{
	/// <summary>
	/// Represents opcodes which reference array elements
	/// </summary>
	public sealed class ElementReferenceOpcode : Opcode
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
			get { return ReflectionEmitEnums.GetLocationReferenceKind(ref opcode); }
		}

		/// <summary>
		/// Gets the CLI data type specified by this opcode, if any.
		/// </summary>
		public DataType? DataType
		{
			get { return dataType; }
		}
		#endregion
	}
}
