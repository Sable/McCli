using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	public sealed class MComplexType : MType
	{
		#region Fields
		public static readonly MComplexType Double = new MComplexType(MNumericClass.Double);
		public static readonly MComplexType Single = new MComplexType(MNumericClass.Single);
		public static readonly MComplexType Int8 = new MComplexType(MNumericClass.Int8);
		public static readonly MComplexType Int16 = new MComplexType(MNumericClass.Int16);
		public static readonly MComplexType Int32 = new MComplexType(MNumericClass.Int32);
		public static readonly MComplexType Int64 = new MComplexType(MNumericClass.Int64);
		public static readonly MComplexType UInt8 = new MComplexType(MNumericClass.UInt8);
		public static readonly MComplexType UInt16 = new MComplexType(MNumericClass.UInt16);
		public static readonly MComplexType UInt32 = new MComplexType(MNumericClass.UInt32);
		public static readonly MComplexType UInt64 = new MComplexType(MNumericClass.UInt64);

		private readonly MNumericClass @class;
		private readonly Type type;
		private readonly Type arrayType;
		#endregion

		#region Constructors
		private MComplexType(MNumericClass @class)
		{
			Contract.Requires(@class != null);
			
			this.@class = @class;
			type = typeof(MComplex<>).MakeGenericType(@class.Type);
			arrayType = typeof(MArray<>).MakeGenericType(type);
		}
		#endregion

		#region Properties
		public override MClass Class
		{
			get { return @class; }
		}

		public Type Type
		{
			get { return type; }
		}

		public Type ArrayType
		{
			get { return arrayType; }
		}

		public override Type BoxedType
		{
			get { return arrayType; }
		}
		#endregion

		#region Methods
		public override bool Equals(MType other)
		{
			return ReferenceEquals(other, this);
		}

		public override int GetHashCode()
		{
			return @class.GetHashCode() ^ unchecked((int)0x80000000);
		}
		#endregion
	}
}
