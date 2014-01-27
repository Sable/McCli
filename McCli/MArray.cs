using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// Base class for all Matlab arrays,
	/// including numeric types, strings and such.
	/// </summary>
	public abstract class MArray : MValue
	{
		#region Fields
		protected MArrayShape shape;
		#endregion

		#region Constructors
		internal MArray(MArrayShape shape)
		{
			this.shape = shape;
		}
		#endregion

		#region Properties
		public Array BackingArray
		{
			get { return GetBackingArray(); }
		}

		public int ElementCount
		{
			get { return shape.TotalCount; }
		}

		public bool IsScalar
		{
			get { return shape.IsScalar; }
		}

		public MArrayShape Shape
		{
			get { return shape; }
		}
		#endregion

		#region Indexers
		public object this[int index]
		{
			get { return At(index); }
		}
		#endregion

		#region Methods
		protected abstract Array GetBackingArray();
		protected abstract object At(int index);
		#endregion
	}

	public sealed class MArray<T> : MArray
	{
		#region Fields
		private T[] elements;
		#endregion

		#region Constructors
		public MArray(T[] backingArray, MArrayShape shape)
			: base(shape)
		{
			Contract.Requires(backingArray != null && backingArray.Length == shape.TotalCount);
			this.elements = backingArray;
		}

		public MArray(MArrayShape shape) : base(shape)
		{
			elements = new T[shape.TotalCount];
		}
		#endregion

		#region Properties
		public new T[] BackingArray
		{
			get { return elements; }
		}

		public override MType MType
		{
			get { throw new NotImplementedException(); }
		}
		#endregion

		#region Indexers
		public new T this[int index]
		{
			get { return elements[index]; }
			set { elements[index] = value; }
		}
		#endregion

		#region Methods
		public static MArray<T> CreateScalar(T value)
		{
			return new MArray<T>(new[] { value }, MArrayShape.Scalar);
		}

		protected override Array GetBackingArray()
		{
			return elements;
		}

		protected override object At(int index)
		{
			return this[index];
		}
		#endregion
	}
}
