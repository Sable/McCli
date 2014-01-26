using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	partial struct ImmutableArray<T>
	{
		public sealed class Builder
		{
			#region Fields
			private T[] array;
			#endregion

			#region Constructors
			public Builder(int length)
			{
				Contract.Requires(length >= 0);

				array = new T[length];
			}
			#endregion

			#region Properties
			public int Length
			{
				get { return array.Length; }
			}
			#endregion

			#region Indexers
			public T this[int index]
			{
				get { return array[index]; }
				set { array[index] = value; }
			}
			#endregion

			#region Methods
			public ImmutableArray<T> Complete()
			{
				var result = new ImmutableArray<T>(array, default(NoCloneTag));
				array = null;
				return result;
			}
			#endregion
		}
	}
}
