using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
	[DebuggerDisplay("Length = {Length}")]
	public partial struct ImmutableArray<T> : IList<T>
	{
		#region Fields
		public static readonly ImmutableArray<T> Empty = CreateNoClone(EmptyArray<T>.Rank1);

		internal readonly T[] array;
		private readonly int length;
		#endregion

		#region Constructors
		private ImmutableArray(T[] array)
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
			return new ImmutableArray<T>(subarray);
		}

		public ImmutableArray<U> Select<U>(Func<T, U> selector)
		{
			Contract.Requires(selector != null);
			var target = new U[length];
			for (int i = 0; i < target.Length; ++i)
				target[i] = selector(array[i]);
			return ImmutableArray<U>.CreateNoClone(target);
		}

		public Enumerator GetEnumerator()
		{
			return new Enumerator(array);
		}

		internal static ImmutableArray<T> CreateNoClone(T[] array)
		{
			Contract.Requires(array != null);
			return new ImmutableArray<T>(array);
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
		public static implicit operator ImmutableArray<T>(ImmutableArray.EmptyType empty)
		{
			return ImmutableArray<T>.Empty;
		}

		public static implicit operator ImmutableArray<T>(T[] array)
		{
			if (array == null) throw new ArgumentNullException("array");
			return new ImmutableArray<T>((T[])array.Clone());
		}

		public static implicit operator ImmutableArray<T>(T singleton)
		{
			return new ImmutableArray<T>(new[] { singleton });
		}
		#endregion
	}

	public static class ImmutableArray
	{
		public struct EmptyType { }

		public static readonly EmptyType Empty;

		public static ImmutableArray<U> Cast<U, T>(ImmutableArray<T> array) where T : U
		{
			return ImmutableArray<U>.CreateNoClone((U[])(object)array.array);
		}

		public static ImmutableArray<T> Create<T>(T element)
		{
			return ImmutableArray<T>.CreateNoClone(new[] { element });
		}

		public static ImmutableArray<T> Create<T>(T element0, T element1)
		{
			return ImmutableArray<T>.CreateNoClone(new[] { element0, element1 });
		}

		public static ImmutableArray<T> Create<T>(T element0, T element1, T element2)
		{
			return ImmutableArray<T>.CreateNoClone(new[] { element0, element1, element2 });
		}

		public static ImmutableArray<T> Create<T>(params T[] elements)
		{
			Contract.Requires(elements != null);
			return ImmutableArray<T>.CreateNoClone((T[])elements.Clone());
		}

		public static ImmutableArray<T> Create<T>(IEnumerable<T> elements)
		{
			Contract.Requires(elements != null);
			return ImmutableArray<T>.CreateNoClone(elements.ToArray());
		}

		public static ImmutableArray<T> ToImmutable<T>(this T[] array)
		{
			Contract.Requires(array != null);
			return ImmutableArray<T>.CreateNoClone(array);
		}

		public static ImmutableArray<T> ToImmutableArray<T>(this IEnumerable<T> sequence)
		{
			Contract.Requires(sequence != null);
			return ImmutableArray<T>.CreateNoClone(sequence.ToArray());
		}
	}
}
