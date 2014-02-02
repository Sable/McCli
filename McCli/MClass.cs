using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// Represents a MatLab class.
	/// Instances are immutable and reference comparable.
	/// </summary>
	public abstract class MClass
	{
		#region Fields
		public static readonly MFunctionHandleClass FunctionHandle = new MFunctionHandleClass();
		public static readonly MPrimitiveClass Char = new MPrimitiveClass(typeof(char), "char");
		public static readonly MPrimitiveClass Bool = new MPrimitiveClass(typeof(bool), "logical");
		public static readonly MPrimitiveClass Double = new MPrimitiveClass(typeof(double), "double");
		public static readonly MPrimitiveClass Single = new MPrimitiveClass(typeof(float), "single");
		public static readonly MPrimitiveClass Int8 = new MPrimitiveClass(typeof(sbyte), "int8");
		public static readonly MPrimitiveClass Int16 = new MPrimitiveClass(typeof(short), "int16");
		public static readonly MPrimitiveClass Int32 = new MPrimitiveClass(typeof(int), "int32");
		public static readonly MPrimitiveClass Int64 = new MPrimitiveClass(typeof(long), "int64");
		public static readonly MPrimitiveClass UInt8 = new MPrimitiveClass(typeof(byte), "uint8");
		public static readonly MPrimitiveClass UInt16 = new MPrimitiveClass(typeof(ushort), "uint16");
		public static readonly MPrimitiveClass UInt32 = new MPrimitiveClass(typeof(uint), "uint32");
		public static readonly MPrimitiveClass UInt64 = new MPrimitiveClass(typeof(ulong), "uint64");
		#endregion

		#region Constructors
		internal MClass() { }
		#endregion

		#region Properties
		/// <summary>
		/// Gets the CLI type that represents basic scalars of this class (without attributes).
		/// </summary>
		public abstract Type BasicScalarType { get; }

		/// <summary>
		/// Gets the name of this class.
		/// </summary>
		public abstract string Name { get; }

		/// <summary>
		/// Gets the default form of values with this class.
		/// </summary>
		public abstract MTypeForm DefaultForm { get; }

		/// <summary>
		/// Gets the fixed size in bytes of values with this class,
		/// or <c>0</c> if value sizes is not fixed.
		/// </summary>
		public abstract int FixedSizeInBytes { get; }
		#endregion

		#region Methods
		/// <summary>
		/// Tests if this class supports a given form.
		/// </summary>
		/// <param name="form">The form to be tested.</param>
		/// <returns>A value indicating if this class supports that form.</returns>
		[Pure]
		public abstract bool SupportsForm(MTypeForm form);

		public Type GetRuntimeType(MTypeForm form, bool complex)
		{
			Contract.Requires(SupportsForm(form));
			Contract.Requires(!complex || this is MPrimitiveClass);

			var type = BasicScalarType;
			if (complex) type = typeof(MComplex<>).MakeGenericType(type);

			if (form == MTypeForm.Array) type = typeof(MArray<>).MakeGenericType(type);
			else if (form != MTypeForm.Scalar) throw new NotImplementedException();

			return type;
		}

		public static MClass FromBasicScalarType(Type type)
		{
			Contract.Requires(type != null);

			if (type == typeof(double)) return MPrimitiveClass.Double;
			throw new NotImplementedException();
		}
		#endregion
	}
}
