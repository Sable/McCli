﻿using System;
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
	public abstract class MClass : MType
	{
		#region Fields
		public static readonly MPrimitiveClass Double = new MPrimitiveClass(MClassKinds.Double, typeof(double));
		public static readonly MPrimitiveClass Single = new MPrimitiveClass(MClassKinds.Single, typeof(float));
		public static readonly MPrimitiveClass Int8 = new MPrimitiveClass(MClassKinds.Int8, typeof(sbyte));
		public static readonly MPrimitiveClass Int16 = new MPrimitiveClass(MClassKinds.Int16, typeof(short));
		public static readonly MPrimitiveClass Int32 = new MPrimitiveClass(MClassKinds.Int32, typeof(int));
		public static readonly MPrimitiveClass Int64 = new MPrimitiveClass(MClassKinds.Int64, typeof(long));
		public static readonly MPrimitiveClass UInt8 = new MPrimitiveClass(MClassKinds.UInt8, typeof(byte));
		public static readonly MPrimitiveClass UInt16 = new MPrimitiveClass(MClassKinds.UInt16, typeof(ushort));
		public static readonly MPrimitiveClass UInt32 = new MPrimitiveClass(MClassKinds.UInt32, typeof(uint));
		public static readonly MPrimitiveClass UInt64 = new MPrimitiveClass(MClassKinds.UInt64, typeof(ulong));
		public static readonly MPrimitiveClass Char = new MPrimitiveClass(MClassKinds.Char, typeof(char));
		public static readonly MPrimitiveClass Logical = new MPrimitiveClass(MClassKinds.Logical, typeof(bool));
		public static readonly MCellClass Cell = new MCellClass();
		//public static readonly MFunctionHandleClass FunctionHandle = new MFunctionHandleClass();
		#endregion

		#region Constructors
		internal MClass() { }
		#endregion

		#region Properties
		/// <summary>
		/// Gets the <see cref="MClassKinds"/> enumerant for this class.
		/// </summary>
		public abstract MClassKinds Kind { get; }
		#endregion

		#region Methods
		protected override sealed MClass GetClass()
		{
			return this;
		}

		public static MClass FromKind(MClassKinds kind)
		{
			return MTypeLookup.ByClassKind(kind);
		}

		public static MClass FromName(string name)
		{
			Contract.Requires(name != null);
			return MTypeLookup.ByClassName(name);
		}
		#endregion
	}
}
