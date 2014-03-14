using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CliKit.Cil
{
	/// <summary>
	/// Pseudo-union type that represents an operand's value.
	/// </summary>
	public struct NumericalOperand
	{
		#region Fields
		[StructLayout(LayoutKind.Explicit, Size = sizeof(long))]
		private struct Value
		{
			[FieldOffset(0)] public long Int64;
			[FieldOffset(0)] public double Float64;
			[FieldOffset(0)] public uint UInt32;
			[FieldOffset(0)] public int Int32;
			[FieldOffset(0)] public float Float32;
		}

		private readonly Value value;
		#endregion

		#region Constructors
		public NumericalOperand(long value)
		{
			this.value = new Value { Int64 = value };
		}

		public NumericalOperand(double value)
		{
			this.value = new Value { Float64 = value };
		}

		public NumericalOperand(int value)
		{
			this.value = new Value { Int32 = value };
		}

		public NumericalOperand(uint value)
		{
			this.value = new Value { UInt32 = value };
		}

		public NumericalOperand(MetadataToken value)
		{
			this.value = new Value { Int32 = value.Value };
		}

		public NumericalOperand(float value)
		{
			this.value = new Value { Float32 = value };
		}
		#endregion

		#region Properties
		public long Int64
		{
			get { return value.Int64; }
		}

		public double Float64
		{
			get { return value.Float64; }
		}

		public int Int
		{
			get { return value.Int32; }
		}

		public uint UInt
		{
			get { return unchecked((uint)value.Int32); }
		}

		public MetadataToken MetadataToken
		{
			get { return MetadataToken.FromValue(value.Int32); }
		}

		public float Float32
		{
			get { return value.Float32; }
		}
		#endregion

		#region Operators
		public static implicit operator NumericalOperand(long value)
		{
			return new NumericalOperand(value);
		}

		public static implicit operator NumericalOperand(double value)
		{
			return new NumericalOperand(value);
		}

		public static implicit operator NumericalOperand(int value)
		{
			return new NumericalOperand(value);
		}

		public static implicit operator NumericalOperand(MetadataToken value)
		{
			return new NumericalOperand(value);
		}

		public static implicit operator NumericalOperand(float value)
		{
			return new NumericalOperand(value);
		}
		#endregion
	}
}
