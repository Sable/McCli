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
		private readonly MPrimitiveForm primitiveForm;
		#endregion

		#region Constructors
		public MRepr(MType type, MPrimitiveForm primitiveForm)
		{
			Contract.Requires(type == null || !type.IsPrimitive || primitiveForm != null);

			this.type = type;
			this.primitiveForm = primitiveForm;
		}

		public MRepr(MType type)
		{
			this.type = type;
			primitiveForm = type == null || !type.IsPrimitive ? null : MPrimitiveForm.Array;
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
		/// Gets a value indicating if this is a representation of a primitive type.
		/// </summary>
		public bool IsPrimitive
		{
			get { return primitiveForm != null; }
		}

		/// <summary>
		/// For primitives, returns the form in which it is represented.
		/// </summary>
		public MPrimitiveForm PrimitiveForm
		{
			get { return primitiveForm; }
		}

		public bool IsMValue
		{
			get { return primitiveForm == null || primitiveForm.IsArray; }
		}

		public bool IsArray
		{
			get { return primitiveForm != null && primitiveForm.IsArray; }
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
				return primitiveForm == null ? type.CliType : primitiveForm.GetCliType(type);
			}
		}
		#endregion

		#region Methods
		public MRepr WithPrimitiveForm(MPrimitiveForm form)
		{
			Contract.Requires(IsPrimitive);
			Contract.Requires(form != null);

			return new MRepr(type, form);
		}

		public bool Equals(MRepr other)
		{
			return type == other.type && primitiveForm == other.primitiveForm;
		}

		public override bool Equals(object obj)
		{
			return obj is MRepr && Equals((MRepr)obj);
		}

		public override int GetHashCode()
		{
			return (type == null ? 0 : type.GetHashCode())
				^ (primitiveForm == null ? 0 : primitiveForm.GetHashCode());
		}

		public override string ToString()
		{
			if (type == null) return "any";
			return primitiveForm == null ? type.Name : (type.Name + ' ' + primitiveForm.Name);
		}

		public static MRepr FromCliType(Type type)
		{
			Contract.Requires(type != null);

			var mtype = MType.FromCliType(type);
			if (mtype != null) return new MRepr(mtype, mtype.IsPrimitive ? MPrimitiveForm.Scalar : null);

			if (!type.IsGenericType) return Any;

			mtype = MType.FromCliType(type.GetGenericArguments()[0]);
			if (mtype == null) return Any;

			var genericTypeDefinition = type.GetGenericTypeDefinition();
			if (genericTypeDefinition == typeof(MArray<>)) return new MRepr(mtype, MPrimitiveForm.Array);
			if (genericTypeDefinition == typeof(MDenseArray<>)) return new MRepr(mtype, MPrimitiveForm.DenseArray);
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
