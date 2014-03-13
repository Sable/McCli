using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// Represents the way a value is represented at runtime.
	/// </summary>
	public struct MRepr : IEquatable<MRepr>
	{
		#region Fields
		public static readonly MRepr Any = default(MRepr);
		
		private readonly MType type;
		private readonly MStructuralClass structuralClass;
		#endregion

		#region Constructors
		public MRepr(MType type, MStructuralClass structuralClass)
		{
			Contract.Requires(type == null || structuralClass != null);

			Contract.Assert(structuralClass != MStructuralClass.Array);
			this.type = type;
			this.structuralClass = structuralClass;
		}

		public MRepr(MType type)
		{
			this.type = type;
			structuralClass = type == null ? null : MStructuralClass.FullArray;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the type underlying this representation.
		/// </summary>
		public MType Type
		{
			get { return type; }
		}

		/// <summary>
		/// Gets the MatLab class of values with this representation.
		/// </summary>
		public MClass Class
		{
			get { return type == null ? null : type.Class; }
		}

		/// <summary>
		/// Gets a value indicating if this instance represents the "any" type.
		/// </summary>
		public bool IsAny
		{
			get { return type == null; }
		}

		/// <summary>
		/// Gets a value indicating if this is a representation of a scalar primitive type.
		/// </summary>
		public bool IsPrimitiveScalar
		{
			get { return structuralClass == MStructuralClass.Scalar && type.Class.IsPrimitive; }
		}

		/// <summary>
		/// Gets the structural class of value representations.
		/// </summary>
		public MStructuralClass StructuralClass
		{
			get { return structuralClass; }
		}

		public bool IsMValue
		{
			get { return structuralClass == null || structuralClass.IsMValue; }
		}

		public bool IsArray
		{
			get { return structuralClass != null && structuralClass.IsArray; }
		}

		public bool IsComplex
		{
			get { return type != null && type.IsComplex; }
		}

		public Type CliType
		{
			get
			{
				if (type == null) return typeof(MValue);
				return structuralClass == null ? type.CliType : structuralClass.GetCliType(type);
			}
		}
		#endregion

		#region Methods
		public MRepr WithStructuralClass(MStructuralClass structuralClass)
		{
			Contract.Requires(!IsAny);
			Contract.Requires(structuralClass != null);

			return new MRepr(type, structuralClass);
		}

		public bool IsSubtypeOf(MRepr other)
		{
			return other.IsAny || (type == other.type
				&& (structuralClass == other.structuralClass || other.IsArray));
		}

		public bool Equals(MRepr other)
		{
			return type == other.type && structuralClass == other.structuralClass;
		}

		public override bool Equals(object obj)
		{
			return obj is MRepr && Equals((MRepr)obj);
		}

		public override int GetHashCode()
		{
			return (type == null ? 0 : type.GetHashCode())
				^ (structuralClass == null ? 0 : structuralClass.GetHashCode());
		}

		public override string ToString()
		{
			if (type == null) return "any";
			return structuralClass == null ? type.Name : (type.Name + ' ' + structuralClass.Name);
		}

		public static MRepr FromCliType(Type type)
		{
			Contract.Requires(type != null);

			var mtype = MType.FromCliType(type);
			if (mtype != null) return new MRepr(mtype, mtype.IsPrimitive ? MStructuralClass.Scalar : null);

			if (!type.IsGenericType) return Any;

			mtype = MType.FromCliType(type.GetGenericArguments()[0]);
			if (mtype == null) return Any;

			var genericTypeDefinition = type.GetGenericTypeDefinition();
			if (genericTypeDefinition == typeof(MArray<>)) return new MRepr(mtype, MStructuralClass.Array);
			if (genericTypeDefinition == typeof(MFullArray<>)) return new MRepr(mtype, MStructuralClass.FullArray);
			return Any;
		}
		#endregion

		#region Operators
		public static implicit operator MRepr(MType type)
		{
			return new MRepr(type);
		}

		public static bool operator==(MRepr lhs, MRepr rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(MRepr lhs, MRepr rhs)
		{
			return !lhs.Equals(rhs);
		}
		#endregion
	}
}
