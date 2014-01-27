using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// An array in which the value of elements cannot change.
	/// </summary>
	/// <typeparam name="T">The type of the elements.</typeparam>
	[Serializable]
	public partial struct ImmutableArray<T> : IList<T>, IReadOnlyList<T>
	{
		private struct NoCloneTag { }

		#region Fields
		public static readonly ImmutableArray<T> Empty = new ImmutableArray<T>(new T[0]);

		private readonly T[] array;
		private readonly int length;
		#endregion

		#region Constructors
		public ImmutableArray(params T[] array)
		{
			Contract.Requires(array != null);

			this.array = (T[])array.Clone();
			length = array.Length;
		}

		private ImmutableArray(T[] array, NoCloneTag tag)
		{
			Contract.Requires(array != null);
			this.array = array;
			this.length = array.Length;
		}
		#endregion

		#region Properties
		public int Length
		{
			get { return length; }
		}

		public bool IsEmpty
		{
			get { return length == 0; }
		}
		#endregion

		#region Indexers
		public T this[int index]
		{
			get { return array[index]; }
		}
		#endregion

		#region Methods
		public int IndexOf(T item)
		{
			return array == null ? -1 : Array.IndexOf<T>(array, item);
		}

		public bool Contains(T item)
		{
			return array != null && Array.IndexOf<T>(array, item) >= 0;
		}

		public void CopyRange(int startIndex, int count, T[] destination, int destinationIndex)
		{
			Contract.Requires(startIndex >= 0);
			Contract.Requires(count >= 0);
			Contract.Requires(startIndex + count <= Length);
			Contract.Requires(destination != null);
			Contract.Requires(destinationIndex >= 0);
			Contract.Requires(destinationIndex + count <= destination.Length);

			if (count > 0) Array.Copy(array, startIndex, destination, destinationIndex, count);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			if (this.array != null) this.array.CopyTo(array, arrayIndex);
		}

		public T[] ToArray()
		{
			return length == 0 ? Empty.array : (T[])array.Clone();
		}

		public ImmutableArray<T> Subarray(int startIndex, int count)
		{
			Contract.Requires(startIndex >= 0);
			Contract.Requires(count >= 0);
			Contract.Requires(startIndex + count <= Length);

			T[] subarray = new T[count];
			if (count > 0) Array.Copy(array, startIndex, subarray, 0, count);
			return new ImmutableArray<T>(subarray, default(NoCloneTag));
		}

		public Enumerator GetEnumerator()
		{
			return new Enumerator(array);
		}

		public static ImmutableArray<T> FromCollection(ICollection<T> collection)
		{
			Contract.Requires(collection != null);

			T[] array = new T[collection.Count];
			collection.CopyTo(array, 0);
			return new ImmutableArray<T>(array, default(NoCloneTag));
		}
		#endregion

		#region Explicit Members
		int ICollection<T>.Count
		{
			get { return length; }
		}

		void ICollection<T>.Add(T item)
		{
			throw new NotSupportedException();
		}

		bool ICollection<T>.Remove(T item)
		{
			throw new NotSupportedException();
		}

		void ICollection<T>.Clear()
		{
			throw new NotSupportedException();
		}

		int IReadOnlyCollection<T>.Count
		{
			get { return length; }
		}

		void IList<T>.Insert(int index, T item)
		{
			throw new NotSupportedException();
		}

		void IList<T>.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		T IList<T>.this[int index]
		{
			get { return this[index]; }
			set { throw new NotSupportedException(); }
		}

		bool ICollection<T>.IsReadOnly
		{
			get { return true; }
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion

		#region Operators
		public static implicit operator ImmutableArray<T>(T[] array)
		{
			if (array == null) throw new ArgumentNullException("array");
			return new ImmutableArray<T>(array);
		}
		#endregion
	}
}
