using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// Non-generic interface to access the backing array of a dense array.
	/// </summary>
	public interface IMDenseArray
	{
		/// <summary>
		/// Gets the backing array of the dense array.
		/// </summary>
		Array BackingArray { get; }
	}

	/// <summary>
	/// Strongly typed base class for all MatLab arrays.
	/// </summary>
	/// <typeparam name="TScalar">The scalar type of the array elements.</typeparam>
	public sealed class MDenseArray<TScalar> : MArray<TScalar>, IMDenseArray
	{
		#region Fields
		private TScalar[] elements;
		#endregion

		#region Constructors
		public MDenseArray(TScalar[] backingArray, MArrayShape shape)
			: base(shape)
		{
			Contract.Requires(backingArray != null && backingArray.Length == shape.Count);
			this.elements = backingArray;
		}

		public MDenseArray(MArrayShape shape)
			: base(shape)
		{
			elements = new TScalar[shape.Count];
		}

		public MDenseArray(int rowCount, int columnCount)
			: this(new MArrayShape(rowCount, columnCount)) { }
		#endregion

		#region Properties
		public TScalar[] BackingArray
		{
			get { return elements; }
		}

		protected override MPrimitiveForm PrimitiveForm
		{
			get { return MPrimitiveForm.DenseArray; }
		}
		#endregion

		#region Indexers
		public override TScalar this[int index]
		{
			get { return elements[index]; }
			set { elements[index] = value; }
		}
		#endregion

		#region Methods
		public new MDenseArray<TScalar> DeepClone()
		{
			return new MDenseArray<TScalar>((TScalar[])elements.Clone(), shape);
		}

		public override MDenseArray<TScalar> AsDense()
		{
			return this;
		}

		protected override object At(int index)
		{
			return this[index];
		}

		protected override MValue DoDeepClone()
		{
			return DeepClone();
		}

		public static MDenseArray<TScalar> CreateScalar(TScalar value)
		{
			return new MDenseArray<TScalar>(new[] { value }, MArrayShape.Scalar);
		}
		#endregion

		#region Explicit Members
		Array IMDenseArray.BackingArray
		{
			get { return elements; }
		}
		#endregion
	}
}
