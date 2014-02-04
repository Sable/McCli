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
	public sealed class MCellArray : MArray
	{
		#region Fields
		private MValue[] elements;
		#endregion

		#region Constructors
		public MCellArray(MValue[] elements, MArrayShape shape) : base(shape)
		{
			Contract.Requires(elements != null && elements.Length >= shape.Count);
			this.elements = elements;
		}

		public MCellArray(MArrayShape shape) : base(shape)
		{
			elements = new MValue[shape.Count];
			for (int i = 0; i < elements.Length; ++i)
				elements[i] = new MDenseArray<double>(MArrayShape.Empty);
		}
		#endregion

		#region Properties
		public override MType Type
		{
			get { return MClass.CellArray.AsType(); }
		}
		#endregion

		#region Indexers
		public new MValue this[int index]
		{
			get { return elements[index]; }
			set
			{
				Contract.Requires(value != null);
				elements[index] = value;
			}
		}
		#endregion

		#region Methods
		public new MCellArray DeepClone()
		{
			var cloneElements = new MValue[elements.Length];
			for (int i = 0; i < elements.Length; ++i)
				cloneElements[i] = elements[i].DeepClone();
			return new MCellArray(cloneElements, shape);
		}

		protected override object At(int index)
		{
			return elements[index];
		}

		protected override MValue DoDeepClone()
		{
			return DeepClone();
		}
		#endregion
	}
}
