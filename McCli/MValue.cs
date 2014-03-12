using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// Base class for boxed MatLab variable values.
	/// </summary>
	public abstract class MValue
	{
		#region Fields
		protected MArrayShape shape;
		#endregion

		#region Constructors
		internal MValue(MArrayShape shape)
		{
			this.shape = shape;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the representation of this value.
		/// </summary>
		public abstract MRepr Repr { get; }

		/// <summary>
		/// Gets the class of this value.
		/// </summary>
		public MClass Class
		{
			get { return Repr.Class; }
		}

		/// <summary>
		/// Gets the shape of this value.
		/// </summary>
		public MArrayShape Shape
		{
			get { return shape; }
		}

		/// <summary>
		/// Gets the number of elements in this value.
		/// </summary>
		public int Count
		{
			get { return shape.Count; }
		}

		/// <summary>
		/// Gets the number of column this value has.
		/// </summary>
		public int ColumnCount
		{
			get { return shape.ColumnCount; }
		}

		/// <summary>
		/// Gets the number of rows this value has.
		/// </summary>
		public int RowCount
		{
			get { return shape.RowCount; }
		}

		/// <summary>
		/// Gets a value indicating if this value contains a single element.
		/// </summary>
		public bool IsScalar
		{
			get { return shape.IsScalar; }
		}

		/// <summary>
		/// Gets a value indicating if this value is an empty array.
		/// </summary>
		public bool IsEmpty
		{
			get { return shape.IsEmpty; }
		}

		/// <summary>
		/// Gets a value indicating if this value has a column vector shape.
		/// </summary>
		public bool IsColumnVector
		{
			get { return shape.IsColumnVector; }
		}

		/// <summary>
		/// Gets a value indicating if this value has a row vector shape.
		/// </summary>
		public bool IsRowVector
		{
			get { return shape.IsRowVector; }
		}

		/// <summary>
		/// Gets a value indicating if this value has a row or column vector shape.
		/// </summary>
		public bool IsVector
		{
			get { return shape.IsVector; }
		}

		/// <summary>
		/// Gets a value indicating if this value is a square matrix.
		/// </summary>
		public bool IsSquareMatrix
		{
			get { return shape.IsSquareMatrix; }
		}

		/// <summary>
		/// Gets a value indicating if this value is an array with more than two dimensions.
		/// </summary>
		public bool IsHigherDimensional
		{
			get { return shape.IsHigherDimensional; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Obtains a deep clone of this value.
		/// </summary>
		/// <returns>A deep clone of this value, or the value itself if it is immutable.</returns>
		public MValue DeepClone()
		{
			return DoDeepClone();
		}

		/// <summary>
		/// Resizes this value to the given shape.
		/// </summary>
		/// <param name="newShape">The new shape of the value.</param>
		public abstract void Resize(MArrayShape newShape);

		protected abstract MValue DoDeepClone();
		#endregion
	}
}
