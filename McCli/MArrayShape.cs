using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// An immutable representation of the shape of a matlab array (size along all dimensions).
	/// </summary>
	public struct MArrayShape : IEquatable<MArrayShape>
	{
		private struct Unchecked
		{
			public static readonly Unchecked Instance;
		}

		#region Fields
		public static readonly MArrayShape Empty = new MArrayShape(0, 0, Unchecked.Instance);
		public static readonly MArrayShape Scalar = new MArrayShape(1, 1, Unchecked.Instance);

		private readonly int rowCount, columnCount;
		private int[] dimensions;
		#endregion

		#region Constructors
		public MArrayShape(int rowCount, int columnCount)
		{
			Contract.Requires(rowCount >= 0);
			Contract.Requires(columnCount >= 0);

			this.rowCount = rowCount;
			this.columnCount = columnCount;
			this.dimensions = null;
		}

		public MArrayShape(params int[] dimensions)
		{
			Contract.Requires(dimensions != null && dimensions.Length >= 2);

			int dimensionCount = 0;
			for (int i = 0; i < dimensions.Length; ++i)
			{
				var size = dimensions[i];
				if (size < 0) throw new ArgumentOutOfRangeException();

				if (size != 1) dimensionCount = i + 1;
			}

			if (dimensionCount < 2) dimensionCount = 2;

			this.rowCount = dimensions[0];
			this.columnCount = dimensions[1];
			this.dimensions = new int[dimensionCount];
			for (int i = 0; i < this.dimensions.Length; ++i)
				this.dimensions[i] = dimensions[i];
		}

		private MArrayShape(int rowCount, int columnCount, Unchecked _)
		{
			this.rowCount = rowCount;
			this.columnCount = columnCount;
			this.dimensions = null;
		}

		private MArrayShape(int[] dimensions, Unchecked _)
		{
			this.rowCount = dimensions[0];
			this.columnCount = dimensions[1];
			this.dimensions = dimensions;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the number of rows in this shape.
		/// </summary>
		public int RowCount
		{
			get { return rowCount; }
		}

		/// <summary>
		/// Gets the number of columns in this shape.
		/// </summary>
		public int ColumnCount
		{
			get { return columnCount; }
		}

		/// <summary>
		/// Gets the number of dimensions this shape has (at least 2).
		/// </summary>
		public int DimensionCount
		{
			get { return dimensions == null ? 2 : dimensions.Length; }
		}

		/// <summary>
		/// Gets the total number of elements across all dimensions of this shape.
		/// </summary>
		public int Count
		{
			get
			{
				if (dimensions == null) return rowCount * columnCount;
				int count = 1;
				foreach (int size in dimensions)
					count *= size;
				return count;
			}
		}

		/// <summary>
		/// Gets a value indicating if this shape is empty (contains no elements).
		/// </summary>
		public bool IsEmpty
		{
			get { return Count == 0; }
		}

		/// <summary>
		/// Gets a value indicating if this shape is a scalar (contains a single element).
		/// </summary>
		public bool IsScalar
		{
			get { return rowCount * columnCount == 1 && !IsHigherDimensional; }
		}

		/// <summary>
		/// Gets a value indicating if this shape is a row vector.
		/// </summary>
		public bool IsRowVector
		{
			get { return rowCount == 1 && rowCount == 1 && !IsHigherDimensional; }
		}

		/// <summary>
		/// Gets a value indicating if this shape is a column vector.
		/// </summary>
		public bool IsColumnVector
		{
			get { return columnCount == 1 && !IsHigherDimensional; }
		}

		/// <summary>
		/// Gets a value indicating if this shape is a row or column vector.
		/// </summary>
		public bool IsVector
		{
			get { return (rowCount == 1 || columnCount == 1) && !IsHigherDimensional; }
		}

		/// <summary>
		/// Gets a value indicating if this shape is a square matrix.
		/// </summary>
		public bool IsSquareMatrix
		{
			get { return rowCount == columnCount && !IsHigherDimensional; }
		}

		/// <summary>
		/// Gets a value indicating if this shape has more than two dimensions.
		/// </summary>
		public bool IsHigherDimensional
		{
			get { return dimensions != null && dimensions.Length > 2; }
		}
		#endregion

		#region Methods
		public bool Equals(MArrayShape other)
		{
			if (DimensionCount != other.DimensionCount) return false;
			if (DimensionCount > 2) throw new NotImplementedException();

			return rowCount == other.rowCount && columnCount == other.columnCount;
		}

		public override bool Equals(object obj)
		{
			return obj is MArrayShape && Equals((MArrayShape)obj);
		}

		public override int GetHashCode()
		{
			return rowCount ^ (columnCount << 16);
		}

		/// <summary>
		/// Gets the size along a given dimension.
		/// </summary>
		/// <param name="index">The zero-based index of a dimension.</param>
		/// <returns>The size along that dimension.</returns>
		public int GetDimensionSize(int index)
		{
			Contract.Requires(index >= 0 && index < DimensionCount);
			if (index == 0) return rowCount;
			if (index == 1) return columnCount;
			return dimensions[index];
		}

		/// <summary>
		/// Gets the linearized distance between adjacent elements in a given dimension.
		/// </summary>
		/// <param name="index">The index of the dimension.</param>
		/// <returns>The distance between adjacent elements in that dimension.</returns>
		public int GetDimensionStep(int index)
		{
			Contract.Requires(index >= 0 && index < DimensionCount);
			if (IsHigherDimensional)
			{
				int step = 1;
				for (int i = 0; i < index; ++i)
					step *= dimensions[i];
				return step;
			}
			else
			{
				if (rowCount * columnCount == 0) return 0;
				return index == 0 ? 1 : rowCount;
			}
		}

		public void GetDimensionStepAndSize(int index, out int step, out int size)
		{
			Contract.Requires(index >= 0 && index < DimensionCount);
			if (IsHigherDimensional)
			{
				step = 1;
				for (int i = 0; i < index; ++i)
					step *= dimensions[i];
				size = dimensions[index];
			}
			else
			{
				if (index == 0)
				{
					step = 1;
					size = rowCount;
				}
				else
				{
					step = rowCount;
					size = columnCount;
				}
			}
		}

		/// <summary>
		/// Gets the index of the first dimension whose size is not one.
		/// </summary>
		/// <returns>The index of the first non-singleton dimension, or <c>zero</c> if scalar.</returns>
		public int IndexOfFirstNonSingletonDimension()
		{
			if (dimensions == null)
			{
				if (rowCount != 1) return 0;
				return columnCount == 1 ? 0 : 1;
			}

			for (int i = 0; i < dimensions.Length; ++i)
				if (dimensions[i] != 1)
					return i;
			return 0;
		}

		/// <summary>
		/// Collapses a shape along a given dimension, reducing that dimension to size 1.
		/// </summary>
		/// <param name="original">The shape to be collapsed along a dimension.</param>
		/// <param name="index">The index of the dimension along which to collapse the shape.</param>
		/// <returns>The shape collapsed along the given dimension.</returns>
		public static MArrayShape CollapseDimension(MArrayShape original, int index)
		{
			Contract.Requires(index >= 0 && index < original.DimensionCount);
			if (original.dimensions == null)
			{
				return index == 0
					? new MArrayShape(original.rowCount == 0 ? 0 : 1, original.columnCount)
					: new MArrayShape(original.rowCount, original.columnCount == 0 ? 0 : 1);
			}

			if (original.dimensions[index] <= 1) return original;

			int[] newDimensions = (int[])original.dimensions.Clone();
			newDimensions[index] = 1;
			return new MArrayShape(newDimensions, Unchecked.Instance);
		}

		public static MArrayShape Square(int count)
		{
			Contract.Requires(count >= 0);
			return new MArrayShape(count, count, Unchecked.Instance);
		}

		public static MArrayShape RowVector(int count)
		{
			Contract.Requires(count >= 0);
			return new MArrayShape(1, count, Unchecked.Instance);
		}

		public static MArrayShape ColumnVector(int count)
		{
			Contract.Requires(count >= 0);
			return new MArrayShape(count, 1, Unchecked.Instance);
		}
		#endregion

		#region Operators
		public static bool operator==(MArrayShape lhs, MArrayShape rhs)
		{
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(MArrayShape lhs, MArrayShape rhs)
		{
			return !lhs.Equals(rhs);
		}
		#endregion
	}
}
