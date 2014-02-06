using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// Non-generic interface to access the backing array of a full array.
	/// </summary>
	public interface IMFullArray
	{
		/// <summary>
		/// Gets the backing array of the full array.
		/// </summary>
		Array BackingArray { get; }
	}

	/// <summary>
	/// The default MatLab array type, full or dense (contiguous) arrays.
	/// </summary>
	/// <typeparam name="TScalar">The scalar type of the array elements.</typeparam>
	public sealed class MFullArray<TScalar> : MArray<TScalar>, IMFullArray
	{
		#region Fields
		private TScalar[] elements;
		#endregion

		#region Constructors
		public MFullArray(TScalar[] backingArray, MArrayShape shape)
			: base(shape)
		{
			Contract.Requires(backingArray != null && backingArray.Length == shape.Count);
			this.elements = backingArray;
		}

		public MFullArray(MArrayShape shape)
			: base(shape)
		{
			elements = new TScalar[shape.Count];
		}

		public MFullArray(int rowCount, int columnCount)
			: this(new MArrayShape(rowCount, columnCount)) { }
		#endregion

		#region Properties
		public TScalar[] BackingArray
		{
			get { return elements; }
		}

		protected override MPrimitiveForm PrimitiveForm
		{
			get { return MPrimitiveForm.FullArray; }
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
		public new MFullArray<TScalar> DeepClone()
		{
			return new MFullArray<TScalar>((TScalar[])elements.Clone(), shape);
		}

		public override MFullArray<TScalar> AsFullArray()
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

		public static MFullArray<TScalar> CreateScalar(TScalar value)
		{
			return new MFullArray<TScalar>(new[] { value }, MArrayShape.Scalar);
		}

		public static MFullArray<TScalar> CreateRowVector(params TScalar[] values)
		{
			return new MFullArray<TScalar>(values, MArrayShape.RowVector(values.Length));
		}

		public static MFullArray<TScalar> CreateColumnVector(params TScalar[] values)
		{
			return new MFullArray<TScalar>(values, MArrayShape.ColumnVector(values.Length));
		}
		#endregion

		#region Explicit Members
		Array IMFullArray.BackingArray
		{
			get { return elements; }
		}
		#endregion
	}
}
