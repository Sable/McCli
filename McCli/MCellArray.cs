using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// Represents a MatLab cell array.
	/// </summary>
	public sealed class MCellArray : MArray<MCell>
	{
		#region Fields
		private MCell[] elements;
		#endregion

		#region Constructors
		public MCellArray(MCell[] elements, MArrayShape shape)
			: base(shape)
		{
			Contract.Requires(elements != null && elements.Length >= shape.Count);
			this.elements = elements;
		}

		public MCellArray(MArrayShape shape) : base(shape)
		{
			elements = new MCell[shape.Count];
			for (int i = 0; i < elements.Length; ++i)
				elements[i] = new MFullArray<double>(MArrayShape.Empty);
		}
		#endregion

		#region Properties
		public override MRepr Repr
		{
			get { return MClass.Cell; }
		}
		#endregion

		#region Indexers
		public new MCell this[int index]
		{
			get { return elements[index].Initialize(); }
			set { elements[index] = value; }
		}
		#endregion

		#region Methods
		public new MCellArray DeepClone()
		{
			var cloneElements = new MCell[elements.Length];
			for (int i = 0; i < elements.Length; ++i)
				cloneElements[i] = elements[i].DeepClone();
			return new MCellArray(cloneElements, shape);
		}

		public override void Resize(MArrayShape newShape)
		{
			throw new NotImplementedException("MCellArray.Resize");
		}

		protected override MValue DoDeepClone()
		{
			return DeepClone();
		}

		protected override MCell GetAt(int index)
		{
			return this[index];
		}

		protected override void SetAt(int index, MCell value)
		{
			this[index] = value;
		}
		#endregion
	}
}
