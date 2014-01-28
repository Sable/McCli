using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	partial struct ImmutableArray<T>
	{
		public struct Enumerator : IEnumerator<T>
		{
			#region Fields
			private readonly T[] array;
			private int index;
			#endregion

			#region Constructors
			internal Enumerator(T[] array)
			{
				this.array = array ?? ImmutableArray<T>.Empty.array;
				this.index = -1;
			}
			#endregion

			#region Properties
			public T Current
			{
				get { return array[index]; }
			}
			#endregion

			#region Methods
			public bool MoveNext()
			{
				index++;
				return index < array.Length;
			}

			public void Reset()
			{
				index = -1;
			}

			public void Dispose() {}
			#endregion

			#region Explicit Members
			object IEnumerator.Current
			{
				get { return Current; }
			}
			#endregion
		}
	}
}
