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
		private readonly DataTypes dataType;
		#endregion

		#region Constructors
		internal ElementReferenceOpcode(Emit.OpCode opcode) : base(opcode)
		{
			string name = opcode.Name;
			int dotIndex = name.LastIndexOf('.');
			if (dotIndex >= 0)
				dataType = DataTypesEnum.TryParseNameInOpcode(name.Substring(dotIndex + 1));
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
		/// Gets the CLI data type specified by this opcode.
		/// <see cref="DataTypes.None"/> indicate a dependence on the array type.
		/// </summary>
		public DataTypes DataType
		{
			get { return dataType; }
		}
		#endregion
	}
}
