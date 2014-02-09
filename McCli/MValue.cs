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
		/// Gets a value indicating if this value contains a single element.
		/// </summary>
		public bool IsScalar
		{
			get { return shape.IsScalar; }
		}

		/// <summary>
		/// Gets a value indicating if this value is an empty array.
		/// </summary>
		public bool IsEmptyArray
		{
			get { return shape.IsEmpty; }
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

		protected abstract MValue DoDeepClone();
		#endregion
	}
}
