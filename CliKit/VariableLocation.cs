using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CliKit
{
	/// <summary>
	/// Identifies a variable's location as either an indexed parameter or an indexed local variable.
	/// </summary>
	[Serializable]
	public struct VariableLocation : IEquatable<VariableLocation>
	{
		#region Instance
		#region Fields
		private ushort kind;
		private ushort index;
		#endregion

		#region Constructors
		public VariableLocation(VariableKind kind, int index)
		{
			Contract.Requires(index >= 0 && index < ushort.MaxValue);
			
			this.kind = unchecked((ushort)kind);
			this.index = unchecked((ushort)index);
		}
		#endregion

		#region Properties
		public VariableKind Kind
		{
			get { return (VariableKind)kind; }
		}

		public bool IsParameter
		{
			get { return Kind == VariableKind.Parameter; }
		}

		public bool IsLocal
		{
			get { return Kind == VariableKind.Local; }
		}

		public int Index
		{
			get { return index; }
		}
		#endregion

		#region Methods
		public bool Equals(VariableLocation other)
		{
			return kind == other.kind && index == other.index;
		}

		public override int GetHashCode()
		{
			return ((int)kind << 16) | (int)index;
		}

		public override bool Equals(object obj)
		{
			return obj is VariableLocation && Equals((VariableLocation)obj);
		}
		#endregion
		#endregion

		#region Static
		#region Fields
		public static readonly VariableLocation This = Parameter(0);
		#endregion

		#region Methods
		public static VariableLocation Parameter(int index)
		{
			return new VariableLocation(VariableKind.Parameter, index);
		}

		public static VariableLocation Local(int index)
		{
			return new VariableLocation(VariableKind.Local, index);
		}
		#endregion

		#region Operators
		public static bool operator==(VariableLocation lhs, VariableLocation rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(VariableLocation lhs, VariableLocation rhs)
		{
			return !lhs.Equals(rhs);
		}
		#endregion
		#endregion
	}
}
