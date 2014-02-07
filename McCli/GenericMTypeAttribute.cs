using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	[AttributeUsage(AttributeTargets.GenericParameter, AllowMultiple = false)]
	public class GenericMTypeAttribute : Attribute
	{
		#region Fields
		private readonly MClassKinds classKinds;
		private readonly bool allowComplex;
		#endregion

		#region Constructors
		public GenericMTypeAttribute(MClassKinds classKinds, bool allowComplex)
		{
			this.classKinds = classKinds;
			this.allowComplex = allowComplex;
		}
		#endregion

		#region Properties
		public MClassKinds ClassKinds
		{
			get { return classKinds; }
		}

		public bool AllowComplex
		{
			get { return allowComplex; }
		}
		#endregion
	}

	public sealed class AnyPrimitiveAttribute : GenericMTypeAttribute
	{
		public AnyPrimitiveAttribute() : base(MClassKinds.PrimitiveMask, allowComplex: true) { }
	}

	public sealed class AnyRealNumericAttribute : GenericMTypeAttribute
	{
		public AnyRealNumericAttribute() : base(MClassKinds.NumericMask, allowComplex: false) { }
	}

	public sealed class AnyArrayableAttribute : GenericMTypeAttribute
	{
		public AnyArrayableAttribute() : base(MClassKinds.NumericMask | MClassKinds.Struct | MClassKinds.Cell, allowComplex: true) { }
	}
}
