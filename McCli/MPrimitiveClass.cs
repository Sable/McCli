using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// Represents primitive MatLab classes: numerical (floating-point and integral), character and logical.
	/// </summary>
	public sealed class MPrimitiveClass : MClass
	{
		#region Fields;
		private readonly MClassKinds kind;
		private readonly string name;
		private readonly Type cliType;
		private readonly MComplexType complexType;
		#endregion

		#region Constructors
		internal MPrimitiveClass(MClassKinds kind, Type cliType)
		{
			Contract.Requires(name != null);

			this.kind = kind;
			this.cliType = cliType;
			this.name = kind.ToString().ToLowerInvariant();
			if ((kind & MClassKinds.NumericMask) != 0)
				complexType = new MComplexType(this);
		}
		#endregion

		#region Properties
		public override MClassKinds Kind
		{
			get { return kind; }
		}

		public override string Name
		{
			get { return name; }
		}

		public override Type CliType
		{
			get { return cliType; }
		}

		public override int FixedSizeInBytes
		{
			get { return cliType.StructLayoutAttribute.Size; }
		}

		/// <summary>
		/// Gets the complex version of this type.
		/// </summary>
		public MComplexType Complex
		{
			get { return complexType; }
		}

		public bool IsNumeric
		{
			get { return (kind & MClassKinds.NumericMask) != 0; }
		}

		public bool IsFloat
		{
			get { return (kind & MClassKinds.FloatMask) != 0; }
		}

		public bool IsInteger
		{
			get { return (kind & MClassKinds.IntegerMask) != 0; }
		}

		public bool IsSignedInteger
		{
			get { return (kind & MClassKinds.SignedIntegerMask) != 0; }
		}

		public bool IsUnsignedInteger
		{
			get { return (kind & MClassKinds.UnsignedIntegerMask) != 0; }
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
		#endregion
	}
}
