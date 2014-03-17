using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// Strongly-typed base class for MatLab arrays.
	/// </summary>
	/// <typeparam name="TScalar">The type of the array elements.</typeparam>
	public abstract class MArray<TScalar> : MValue
	{
		#region Fields
		protected static readonly MType type;
		#endregion

		#region Constructors
		internal MArray(MArrayShape shape) : base(shape) {}

		static MArray()
		{
			// Ensure the generic type is one that supports being in arrays.
			type = MType.FromCliType(typeof(TScalar));
			Contract.Assert(type != null && type.IsPrimitive);
		}
		#endregion

		#region Indexers
		/// <summary>
		/// Accesses an element in this array from its index.
		/// </summary>
		/// <param name="index">The linearized, zero-based index.</param>
		/// <returns>The element at that index.</returns>
		public TScalar this[int index]
		{
			get { return GetAt(index); }
			set { SetAt(index, value); }
		}
		#endregion

		#region Methods
		public new MArray<TScalar> DeepClone()
		{
			return (MArray<TScalar>)DoDeepClone();
		}

		protected abstract TScalar GetAt(int index);
		protected abstract void SetAt(int index, TScalar value);
		#endregion

		#region Operators
		public static implicit operator MArray<TScalar>(TScalar scalar)
		{
			return MFullArray<TScalar>.CreateScalar(scalar);
		}
		#endregion
	}
}
