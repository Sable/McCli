using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CliKit.Cil
{
	/// <summary>
	/// Identifies a kind of numerical conversion.
	/// </summary>
	[Serializable]
	public enum Conversion
	{
		/// <summary>
		/// Indicates that no conversion needs to take place.
		/// </summary>
		Identity,

		/// <summary>
		/// Indicates that some of an integer's most significant bits are dropped.
		/// </summary>
		Truncate,

		/// <summary>
		/// Indicates that the integer is widened by adding zeroes.
		/// </summary>
		ZeroExtend,

		/// <summary>
		/// Indicates that the integer is widened by duplicating its sign bit.
		/// </summary>
		SignExtend,

		/// <summary>
		/// Indicates that a floating-point value is converted to an integer value.
		/// </summary>
		FloatToInt,

		/// <summary>
		/// Indicates that an integer value is converted to a floating-point value.
		/// </summary>
		IntToFloat,

		/// <summary>
		/// Indicates that a floating-point value is reencoded with a different precision.
		/// </summary>
		ChangePrecision
	}
}
