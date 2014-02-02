using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// Base class for all MatLab arrays,
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
		public new MArray DeepClone()
		{
			return (MArray)DoDeepClone();
		}

		protected abstract Array GetBackingArray();
		protected abstract object At(int index);
		#endregion
	}

	/// <summary>
	/// Strongly typed base class for all MatLab arrays.
	/// </summary>
	/// <typeparam name="T">The type of the array elements.</typeparam>
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

		public MArray(int rowCount, int columnCount)
			: this(new MArrayShape(rowCount, columnCount)) { }
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
		public T ToScalar()
		{
			if (!IsScalar) throw new InvalidCastException();
			return elements[0];
		}

		public new MArray<T> DeepClone()
		{
			return new MArray<T>((T[])elements.Clone(), shape);
		}

		protected override Array GetBackingArray()
		{
			return elements;
		}

		protected override object At(int index)
		{
			return this[index];
		}

		protected override MValue DoDeepClone()
		{
			return DeepClone();
		}

		public static MArray<T> CreateScalar(T value)
		{
			return new MArray<T>(new[] { value }, MArrayShape.Scalar);
		}
		#endregion
	}
}
