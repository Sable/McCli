using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emit = System.Reflection.Emit;

namespace CliKit
{
	/// <summary>
	/// Represents a metadata token.
	/// </summary>
	public struct MetadataToken : IEquatable<MetadataToken>
	{
		#region Fields
		private readonly int value;
		#endregion

		#region Constructors
		private MetadataToken(int value)
		{
			this.value = value;
		}
		#endregion

		#region Properties
		public int Value
		{
			get { return value; }
		}

		public MetadataTokenKind Kind
		{
			get { return (MetadataTokenKind)((value >> 24) & 0xFF); }
		}

		public bool IsNil
		{
			get { return (value & 0xFFFFFF) == 0; }
		}

		public int Index
		{
			get { return (value & 0xFFFFFF) - 1; }
		}
		#endregion

		#region Methods
		public bool Equals(MetadataToken other)
		{
			return value == other.value;
		}

		public override bool Equals(object obj)
		{
			return obj is MetadataToken && Equals((MetadataToken)obj);
		}

		public override int GetHashCode()
		{
			return value;
		}

		public override string ToString()
		{
			return string.Concat(Kind, IsNil ? " nil" : (" " + Index));
		}

		public static MetadataToken FromValue(int value)
		{
			return new MetadataToken(value);
		}

		public static MetadataToken FromKindIndex(MetadataTokenKind kind, int index)
		{
			Contract.Requires(index >= -1 && index <= 0xFFFFFE);
			return new MetadataToken(((int)kind << 24) | (index + 1));
		}

		public static MetadataToken Nil(MetadataTokenKind kind)
		{
			return new MetadataToken((int)kind << 24);
		}
		#endregion

		#region Operators
		public static bool operator ==(MetadataToken lhs, MetadataToken rhs) { return lhs.Equals(rhs); }
		public static bool operator !=(MetadataToken lhs, MetadataToken rhs) { return !lhs.Equals(rhs); }

		public static implicit operator MetadataToken(Emit.EventToken token) { return FromValue(token.Token); }
		public static implicit operator MetadataToken(Emit.FieldToken token) { return FromValue(token.Token); }
		public static implicit operator MetadataToken(Emit.MethodToken token) { return FromValue(token.Token); }
		public static implicit operator MetadataToken(Emit.ParameterToken token) { return FromValue(token.Token); }
		public static implicit operator MetadataToken(Emit.PropertyToken token) { return FromValue(token.Token); }
		public static implicit operator MetadataToken(Emit.SignatureToken token) { return FromValue(token.Token); }
		public static implicit operator MetadataToken(Emit.StringToken token) { return FromValue(token.Token); }
		public static implicit operator MetadataToken(Emit.TypeToken token) { return FromValue(token.Token); }
		#endregion
	}
}
