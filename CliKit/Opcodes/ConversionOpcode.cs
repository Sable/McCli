using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emit = System.Reflection.Emit;

namespace CliKit
{
	/// <summary>
	/// Represents opcodes which perform numerical conversions.
	/// </summary>
	public sealed class ConversionOpcode : Opcode
	{
		#region Fields
		private readonly DataTypes targetDataType;
		private readonly bool isSourceUnsigned;
		private readonly bool isOverflowChecked;
		#endregion

		#region Constructors
		internal ConversionOpcode(Emit.OpCode opcode) : base(opcode)
		{
			string opcodeName = opcode.Name;
			isSourceUnsigned = opcodeName.EndsWith(".un");
			isOverflowChecked = opcodeName.StartsWith("conv.ovf.");
		}
		#endregion

		#region Properties
		public DataTypes ValidSourceDataTypes
		{
			get { throw new NotImplementedException(); }
		}

		public DataTypes TargetDataType
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsSourceUnsigned
		{
			get { return isSourceUnsigned; }
		}

		public bool IsOverflowChecked
		{
			get { return isOverflowChecked; }
		}
		#endregion
	}
}
