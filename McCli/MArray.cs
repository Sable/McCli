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
		public MArrayShape Shape
		{
			get { return shape; }
		}

		public int Count
		{
			get { return shape.TotalCount; }
		}

		public bool IsScalar
		{
			get { return shape.IsScalar; }
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

		protected abstract object At(int index);
		#endregion
	}

	/// <summary>
	/// Strongly-typed base class for MatLab arrays.
	/// </summary>
	/// <typeparam name="TScalar">The type of the array elements.</typeparam>
	public abstract class MArray<TScalar> : MArray
	{
		#region Constructors
		internal MArray(MArrayShape shape) : base(shape) {}
		#endregion

		#region Properties
		public override MClass Class
		{
			get { throw new NotImplementedException(); }
		}
		#endregion

		#region Indexers
		public new abstract TScalar this[int index] { get; set; }

		public override MClassAttributes ClassAttributes
		{
			get
			{
				bool isComplex = typeof(TScalar).IsGenericType && typeof(TScalar).GetGenericTypeDefinition() == typeof(MComplex<>);
				return isComplex ? MClassAttributes.Complex : MClassAttributes.None;
			}
		}
		#endregion

		#region Methods
		public TScalar ToScalar()
		{
			if (!IsScalar) throw new InvalidCastException();
			return this[0];
		}

		public abstract MDenseArray<TScalar> AsDense();

		protected override object At(int index)
		{
			return this[index];
		}
		#endregion
	}
}
