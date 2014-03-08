using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// Identifies a generic parameter of a MatLab builtin function
	/// as being instantiatable with a set of types.
	/// </summary>
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

	public sealed class AnyNumericAttribute : GenericMTypeAttribute
	{
		public AnyNumericAttribute() : base(MClassKinds.NumericMask, allowComplex: true) { }
	}

	public sealed class AnyRealAttribute : GenericMTypeAttribute
	{
		public AnyRealAttribute() : base(MClassKinds.NumericMask, allowComplex: false) { }
	}
}
