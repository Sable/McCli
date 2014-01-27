using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	public abstract class MType : IEquatable<MType>
	{
		#region Constructors
		internal MType() { }
		#endregion

		#region Properties
		/// <summary>
		/// Gets the underlying class of this type.
		/// </summary>
		public abstract MClass Class { get; }
		public abstract Type BoxedType { get; }
		#endregion

		#region Methods
		public abstract bool Equals(MType other);

		public override sealed bool Equals(object obj)
		{
			return Equals(obj as MType);
		}

		public override abstract int GetHashCode();

		public static bool Equals(MType first, MType second)
		{
			return ReferenceEquals(first, null)
				? ReferenceEquals(second, null)
				: first.Equals(second);
		}
		#endregion

		#region Operators
		public static bool operator==(MType lhs, MType rhs)
		{
			return Equals(lhs, rhs);
		}

		public static bool operator !=(MType lhs, MType rhs)
		{
			return !Equals(lhs, rhs);
		}
		#endregion
	}
}
