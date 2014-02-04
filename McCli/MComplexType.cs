using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// Represents the complex version of a numerical class.
	/// </summary>
	public sealed class MComplexType : MType
	{
		#region Fields
		private readonly MPrimitiveClass @class;
		private readonly Type cliType;
		#endregion

		#region Constructors
		internal MComplexType(MPrimitiveClass @class)
		{
			Contract.Requires(@class != null && (@class.Kind & MClassKinds.SupportsComplexMask) != 0);

			this.@class = @class;
			this.cliType = typeof(MComplex<>).MakeGenericType(@class.CliType);
		}
		#endregion

		#region Properties
		public new MPrimitiveClass Class
		{
			get { return @class; }
		}

		public override string Name
		{
			get { return "complex " + @class.Name; }
		}

		public override Type CliType
		{
			get { return cliType; }
		}

		public override int FixedSizeInBytes
		{
			get { return @class.FixedSizeInBytes * 2; }
		}

		public MRepr ScalarRepr
		{
			get { return new MRepr(this, MPrimitiveForm.Scalar); }
		}

		public MRepr ArrayRepr
		{
			get { return new MRepr(this, MPrimitiveForm.Array); }
		}

		public MRepr DenseArrayRepr
		{
			get { return new MRepr(this, MPrimitiveForm.DenseArray); }
		}
		#endregion

		#region Methods
		protected override MClass GetClass()
		{
			return @class;
		}
		#endregion
	}
}
