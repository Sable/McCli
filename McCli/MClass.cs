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
		public static readonly MPrimitiveClass Char = new MPrimitiveClass(typeof(char), "char");
		public static readonly MPrimitiveClass Bool = new MPrimitiveClass(typeof(bool), "logical");
		public static readonly MCellArrayClass CellArray = new MCellArrayClass();
		public static readonly MFunctionHandleClass FunctionHandle = new MFunctionHandleClass();

		private static readonly Dictionary<Type, MClass> cliTypeToClass;
		#endregion

		#region Constructors
		internal MClass() { }

		static MClass()
		{
			cliTypeToClass = new Dictionary<Type,MClass>();

			foreach (var field in typeof(MClass).GetFields())
			{
				if (field.IsStatic && field.FieldType == typeof(MPrimitiveClass))
				{
					var primitiveClass = (MPrimitiveClass)field.GetValue(null);
					cliTypeToClass.Add(primitiveClass.CliType, primitiveClass);
				}
			}

			cliTypeToClass.Add(typeof(MCellArray), CellArray);
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the name of this class.
		/// </summary>
		public abstract string Name { get; }

		/// <summary>
		/// Gets the attributes supported by this class.
		/// </summary>
		public abstract MClassAttributes SupportedAttributes { get; }
		#endregion

		#region Methods
		/// <summary>
		/// Gets the size of a scalar of this class with the specified attributes, in bytes.
		/// </summary>
		/// <param name="attributes">The class attributes.</param>
		/// <returns>The size in bytes of a scalar.</returns>
		public abstract int GetScalarSizeInBytes(MClassAttributes attributes);

		/// <summary>
		/// Obtains the MatLab class represented by a given CLI type.
		/// </summary>
		/// <param name="type">The CLI type.</param>
		/// <param name="attributes">Outputs the class attributes of the representation.</param>
		/// <returns>The MatLab class represented by that CLI type, if any.</returns>
		public static MClass FromCliType(Type type, out MClassAttributes attributes)
		{
			Contract.Requires(type != null);

			attributes = MClassAttributes.None;

			MClass @class;
			while (true)
			{
				if (cliTypeToClass.TryGetValue(type, out @class)) return @class;
				if (!type.IsGenericType) return null;

				if (type.GetGenericTypeDefinition() == typeof(MComplex<>)) attributes |= MClassAttributes.Complex;
				type = type.GetGenericArguments()[0];
			}
		}

		public static MClass FromCliType(Type type)
		{
			MClassAttributes attributes;
			return FromCliType(type, out attributes);
		}
		#endregion
	}
}
