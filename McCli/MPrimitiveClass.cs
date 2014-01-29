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
		public static MPrimitiveClass Char = new MPrimitiveClass(typeof(char), "char");
		public static MPrimitiveClass Bool = new MPrimitiveClass(typeof(bool), "logical");
		public static MPrimitiveClass Double = new MPrimitiveClass(typeof(double), "double");
		public static MPrimitiveClass Single = new MPrimitiveClass(typeof(float), "single");
		public static MPrimitiveClass Int8 = new MPrimitiveClass(typeof(sbyte), "int8");
		public static MPrimitiveClass Int16 = new MPrimitiveClass(typeof(short), "int16");
		public static MPrimitiveClass Int32 = new MPrimitiveClass(typeof(int), "int32");
		public static MPrimitiveClass Int64 = new MPrimitiveClass(typeof(long), "int64");
		public static MPrimitiveClass UInt8 = new MPrimitiveClass(typeof(byte), "uint8");
		public static MPrimitiveClass UInt16 = new MPrimitiveClass(typeof(ushort), "uint16");
		public static MPrimitiveClass UInt32 = new MPrimitiveClass(typeof(uint), "uint32");
		public static MPrimitiveClass UInt64 = new MPrimitiveClass(typeof(ulong), "uint64");

		private readonly Type basicScalarType;
		private readonly string name;
		#endregion

		#region Constructors
		private MPrimitiveClass(Type basicScalarType, string name)
		{
			Contract.Requires(basicScalarType != null);

			this.basicScalarType = basicScalarType;
			this.name = name;
		}
		#endregion

		#region Properties
		public override string Name
		{
			get { return name; }
		}

		public override Type BasicScalarType
		{
			get { return basicScalarType; }
		}

		public bool IsFloat
		{
			get { return basicScalarType == typeof(float) || basicScalarType == typeof(double); }
		}

		public bool IsInteger
		{
			get { return !IsFloat; }
		}

		public bool IsSignedInteger
		{
			get
			{
				return basicScalarType == typeof(sbyte)
					|| basicScalarType == typeof(short)
					|| basicScalarType == typeof(int)
					|| basicScalarType == typeof(long);
			}
		}

		public bool IsUnsignedInteger
		{
			get
			{
				return basicScalarType == typeof(byte)
					|| basicScalarType == typeof(ushort)
					|| basicScalarType == typeof(uint)
					|| basicScalarType == typeof(ulong);
			}
		}

		public override MTypeForm DefaultForm
		{
			get { return MTypeForm.Array; }
		}

		public override int FixedSizeInBytes
		{
			get { return Marshal.SizeOf(basicScalarType); }
		}
		#endregion

		#region Methods
		public override bool SupportsForm(MTypeForm form)
		{
			return true;
		}
		#endregion
	}
}
