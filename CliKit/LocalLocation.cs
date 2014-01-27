using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace CliKit
{
	/// <summary>
	/// Identifies a local's location as either an indexed parameter or an indexed local variable.
	/// </summary>
	[Serializable]
	public struct LocalLocation : IEquatable<LocalLocation>
	{
		#region Instance
		#region Fields
		private ushort kind;
		private ushort index;
		#endregion

		#region Constructors
		public LocalLocation(LocalKind kind, int index)
		{
			Contract.Requires(index >= 0 && index < ushort.MaxValue);
			
			this.kind = unchecked((ushort)kind);
			this.index = unchecked((ushort)index);

			uint foo = 0xFFFFFFFD;
			uint bar = 4;
			uint frob = foo * bar;

			Console.WriteLine(frob);
		}
		#endregion

		#region Properties
		public LocalKind Kind
		{
			get { return (LocalKind)kind; }
		}

		public bool IsParameter
		{
			get { return Kind == LocalKind.Parameter; }
		}

		public bool IsVariable
		{
			get { return Kind == LocalKind.Variable; }
		}

		public int Index
		{
			get { return index; }
		}
		#endregion

		#region Methods
		public bool Equals(LocalLocation other)
		{
			return kind == other.kind && index == other.index;
		}

		public override int GetHashCode()
		{
			return ((int)kind << 16) | (int)index;
		}

		public override bool Equals(object obj)
		{
			return obj is LocalLocation && Equals((LocalLocation)obj);
		}
		#endregion
		#endregion

		#region Static
		#region Fields
		public static readonly LocalLocation This = Parameter(0);
		#endregion

		#region Methods
		public static LocalLocation Parameter(int index)
		{
			return new LocalLocation(LocalKind.Parameter, index);
		}

		public static LocalLocation Variable(int index)
		{
			return new LocalLocation(LocalKind.Variable, index);
		}
		#endregion

		#region Operators
		public static bool operator==(LocalLocation lhs, LocalLocation rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(LocalLocation lhs, LocalLocation rhs)
		{
			return !lhs.Equals(rhs);
		}
		#endregion
		#endregion
	}
}
