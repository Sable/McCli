using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	public abstract class MType
	{
		public abstract string ClassName { get; }
		public abstract Type BoxedType { get; }
	}

	public sealed class MNumericType : MType
	{
		#region Fields
		public static MNumericType Double = new MNumericType(typeof(double), "double");

		private readonly Type classType;
		private readonly Type elementType;
		private readonly Type arrayType;
		private readonly string className;
		#endregion

		#region Constructors
		private MNumericType(Type elementType, string className)
		{
			Contract.Requires(elementType != null);
			this.elementType = elementType;
			this.classType = elementType.IsGenericType
				? elementType.GetGenericArguments()[0] : elementType;
			this.arrayType = typeof(MArray<>).MakeGenericType(elementType);
			this.className = className;
		}
		#endregion

		#region Properties
		public Type ClassType
		{
			get { return classType; }
		}

		public Type ElementType
		{
			get { return elementType; }
		}

		public bool IsComplex
		{
			get { return elementType != classType; }
		}

		public Type ArrayType
		{
			get { return arrayType; }
		}

		public override Type BoxedType
		{
			get { return arrayType; }
		}

		public override string ClassName
		{
			get { return className; }
		}
		#endregion

		#region Methods
		public static MNumericType FromElementType(Type elementType)
		{
			if (elementType == typeof(double)) return Double;
			throw new NotImplementedException();
		}

		public static MNumericType FromElementType<T>()
		{
			return FromElementType(typeof(T));
		}
		
		public static MNumericType FromPrimitiveType(Type primitiveType, bool complex)
		{
			if (complex) primitiveType = typeof(MComplex<>).MakeGenericType(primitiveType);
			return FromElementType(primitiveType);
		}

		public static MNumericType FromPrimitiveType<T>(bool complex)
		{
			return FromPrimitiveType(typeof(T), complex);
		}
		#endregion
	}
}
