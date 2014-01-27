using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	public sealed class MNumericClass : MClass
	{
		#region Fields
		public static MNumericClass Double = new MNumericClass(typeof(double), "double");
		public static MNumericClass Single = new MNumericClass(typeof(float), "single");
		public static MNumericClass Int8 = new MNumericClass(typeof(sbyte), "int8");
		public static MNumericClass Int16 = new MNumericClass(typeof(short), "int16");
		public static MNumericClass Int32 = new MNumericClass(typeof(int), "int32");
		public static MNumericClass Int64 = new MNumericClass(typeof(long), "int64");
		public static MNumericClass UInt8 = new MNumericClass(typeof(byte), "uint8");
		public static MNumericClass UInt16 = new MNumericClass(typeof(ushort), "uint16");
		public static MNumericClass UInt32 = new MNumericClass(typeof(uint), "uint32");
		public static MNumericClass UInt64 = new MNumericClass(typeof(ulong), "uint64");

		private readonly Type type;
		private readonly Type arrayType;
		private readonly string name;
		#endregion

		#region Constructors
		private MNumericClass(Type type, string name)
		{
			Contract.Requires(type != null);

			this.type = type;
			this.name = name;
			this.arrayType = typeof(MArray<>).MakeGenericType(type);
		}
		#endregion

		#region Properties
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

		public override string Name
		{
			get { return name; }
		}
		#endregion
	}
}
