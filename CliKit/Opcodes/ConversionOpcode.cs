using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
		private static readonly Regex nameRegex = new Regex("conv(.ovf)?.([a-z]+)(.un)?", RegexOptions.CultureInvariant);

		private readonly DataType targetDataType;
		private readonly bool isSourceUnsigned;
		private readonly bool isOverflowChecked;
		#endregion

		#region Constructors
		internal ConversionOpcode(Emit.OpCode opcode) : base(opcode)
		{
			var match = nameRegex.Match(opcode.Name);
			targetDataType = DataTypeEnum.TryParseNameInOpcode(match.Groups[2].Value);
			isSourceUnsigned = match.Groups[3].Success;
			isOverflowChecked = match.Groups[1].Success;
		}
		#endregion

		#region Properties
		public DataType TargetDataType
		{
			get { return targetDataType; }
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

		#region Methods
		public bool IsValidSourceDataType(DataType source, out bool verifiable)
		{
			if (source.IsNumericStackType())
			{
				verifiable = true;
				return true;
			}

			if (source.IsReferenceOrManagedPointer() && targetDataType.IsNativeOr64BitsInteger())
			{
				verifiable = false; // Stops GC tracking
				return true;
			}

			verifiable = false;
			return false;
		}
		#endregion
	}
}
