using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CliKit.Cil
{
	/// <summary>
	/// Identifies a variable's location as either an indexed parameter or an indexed local variable.
	/// </summary>
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
			Contract.Requires(IsValidIndex(index));
			
			this.kind = unchecked((ushort)kind);
			this.index = unchecked((ushort)index);
		}
		#endregion

		#region Properties
		public VariableKind Kind
		{
			get { return (VariableKind)kind; }
		}

		public bool IsArgument
		{
			get { return Kind == VariableKind.Argument; }
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

		public override bool Equals(object obj)
		{
			return obj is VariableLocation && Equals((VariableLocation)obj);
		}

		public override int GetHashCode()
		{
			return ((int)kind << 16) | (int)index;
		}

		public override string ToString()
		{
			return (IsArgument ? "argument " : "local ") + index.ToString(CultureInfo.InvariantCulture); 
		}
		#endregion
		#endregion

		#region Static
		#region Fields
		public static readonly VariableLocation This = Parameter(0);
		#endregion

		#region Methods
		[Pure]
		public static bool IsValidIndex(int index)
		{
			return index >= 0 && index < 0xFFFF;
		}

		public static VariableLocation Parameter(int index)
		{
			return new VariableLocation(VariableKind.Argument, index);
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
