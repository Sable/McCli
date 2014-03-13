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
		private static readonly MRepr repr;

		internal TScalar[] elements;
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

		static MFullArray()
		{
			repr = new MRepr(MArray<TScalar>.type, MStructuralClass.FullArray);
		}
		#endregion

		#region Properties
		public TScalar[] BackingArray
		{
			get { return elements; }
		}

		public override MRepr Repr
		{
			get { return repr; }
		}
		#endregion

		#region Indexers
		public new TScalar this[int index]
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

		public override void Resize(MArrayShape newShape)
		{
			var copyShape = MArrayShape.Min(shape, newShape);
			int copyCount = copyShape.Count;

			var newArray = new TScalar[newShape.Count];
			if (copyShape.IsHigherDimensional)
			{
				throw new NotImplementedException("Resizing to or from higher-dimensional arrays.");
			}
			else
			{
				for (int columnIndex = 0; columnIndex < copyShape.ColumnCount; ++columnIndex)
				{
					for (int rowIndex = 0; rowIndex < copyShape.RowCount; ++rowIndex)
					{
						var value = elements[(columnIndex - 1) * shape.RowCount + rowIndex - 1];
						elements[(columnIndex - 1) * newShape.RowCount + rowIndex - 1] = value;
					}
				}
			}

			elements = newArray;
			shape = newShape;
		}

		protected override MValue DoDeepClone()
		{
			return DeepClone();
		}

		protected override TScalar GetAt(int index)
		{
			return this[index];
		}

		protected override void SetAt(int index, TScalar value)
		{
			this[index] = value;
		}

		public static MFullArray<TScalar> CreateEmpty()
		{
			return new MFullArray<TScalar>(EmptyArray<TScalar>.Rank1, MArrayShape.Empty);
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

		#region CreateWithShape
		public static MFullArray<TScalar> CreateWithShape(MArrayShape shape)
		{
			return new MFullArray<TScalar>(shape);
		}

		public static MFullArray<TScalar> CreateWithShape(int rowCount, int columnCount)
		{
			return CreateWithShape(new MArrayShape(rowCount, columnCount));
		}

		public static MFullArray<TScalar> CreateWithShape(double rowCount, double columnCount)
		{
			var shape = MArrayShape.FromDoubles(rowCount, columnCount);
			return CreateWithShape(shape);
		}
		#endregion

		#region ExpandScalar
		public static MFullArray<TScalar> ExpandScalar(TScalar value, MArrayShape shape)
		{
			var array = new TScalar[shape.Count];
			for (int i = 0; i < array.Length; ++i) array[i] = value;
			return new MFullArray<TScalar>(array, shape);
		}
		
		public static MFullArray<TScalar> ExpandScalar(TScalar value, int rowCount, int columnCount)
		{
			return ExpandScalar(value, new MArrayShape(rowCount, columnCount));
		}

		public static MFullArray<TScalar> ExpandScalar(TScalar value, double rowCount, double columnCount)
		{
			var shape = MArrayShape.FromDoubles(rowCount, columnCount);
			return ExpandScalar(value, shape);
		}
		#endregion
		#endregion

		#region Explicit Members
		Array IMFullArray.BackingArray
		{
			get { return elements; }
		}
		#endregion

		#region Operators
		public static implicit operator MFullArray<TScalar>(TScalar scalar)
		{
			return MFullArray<TScalar>.CreateScalar(scalar);
		}
		#endregion
	}
}
