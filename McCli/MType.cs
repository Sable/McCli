using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// Represents a matlab type as a combination of a class and its attributes.
	/// </summary>
	public struct MType : IEquatable<MType>
	{
		#region Fields
		public static readonly MType Any = default(MType);

		private const byte complexFlag = 0x80;
		private const byte formFlagsMask = 3;

		private readonly MClass @class;
		private readonly byte flags;
		#endregion

		#region Constructors
		public MType(MClass @class, MTypeForm form, bool complex)
		{
			Contract.Requires(@class != null);
			Contract.Requires(@class.SupportsForm(form));
			Contract.Requires(!complex || @class is MPrimitiveClass);

			this.@class = @class;
			this.flags = (byte)form;
			if (complex) this.flags |= complexFlag;
		}

		public MType(MClass @class, MTypeForm form)
		{
			Contract.Requires(@class != null);
			Contract.Requires(@class.SupportsForm(form));

			this.@class = @class;
			this.flags = (byte)form;
		}

		public MType(MClass @class, bool complex)
		{
			Contract.Requires(@class != null);
			Contract.Requires(!complex || @class is MPrimitiveClass);

			this.@class = @class;
			this.flags = (byte)@class.DefaultForm;
			if (complex) this.flags |= complexFlag;
		}

		public MType(MClass @class)
		{
			Contract.Requires(@class != null);

			this.@class = @class;
			this.flags = (byte)@class.DefaultForm;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets a value indicating if this type represents the base "any" type.
		/// </summary>
		public bool IsAny
		{
			get { return @class == null; }
		}

		/// <summary>
		/// Gets the underlying class of this type.
		/// <c>null</c> if this represents the "any" type.
		/// </summary>
		public MClass Class
		{
			get { return @class; }
		}

		/// <summary>
		/// Gets the form of this type.
		/// </summary>
		public MTypeForm Form
		{
			get
			{
				Contract.Requires(!IsAny);
				return (MTypeForm)(flags & formFlagsMask);
			}
		}

		public bool IsComplex
		{
			get { return (flags & complexFlag) != 0; }
		}

		public Type CliType
		{
			get { return @class == null ? typeof(MValue) : @class.GetRuntimeType(Form, IsComplex); }
		}
		#endregion

		#region Methods
		public bool Equals(MType other)
		{
			return @class == other.@class && flags == other.flags;
		}

		public override bool Equals(object obj)
		{
			return obj is MType && Equals((MType)obj);
		}

		public override int GetHashCode()
		{
			return (@class == null ? 0 : @class.GetHashCode()) ^ ((int)flags << 24);
		}
		#endregion

		#region Operators
		public static MType FromCliType(Type type)
		{
			Contract.Requires(type != null);
			if (type == typeof(MValue)) return Any;

			var form = MTypeForm.Scalar;
			bool complex = false;
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(MArray<>))
			{
				type = type.GetGenericArguments()[0];
				form = MTypeForm.Array;
			}

			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(MComplex<>))
			{
				type = type.GetGenericArguments()[0];
				complex = true;
			}

			var @class = MClass.FromBasicScalarType(type);
			return new MType(@class, form, complex);
		}

		public static implicit operator MType(MClass @class)
		{
			return new MType(@class);
		}

		public static implicit operator Type(MType type)
		{
			return type.CliType;
		}

		public static bool operator==(MType lhs, MType rhs)
		{
			return Equals(lhs, rhs);
		}

		public static bool operator !=(MType lhs, MType rhs)
		{
			return !Equals(lhs, rhs);
		}
		#endregion
	}
}
