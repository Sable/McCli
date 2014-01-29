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
		private readonly MClass @class;
		private readonly byte form;
		private readonly bool complex;
		#endregion

		#region Constructors
		public MType(MClass @class, MTypeForm form, bool complex)
		{
			Contract.Requires(@class != null);
			Contract.Requires(@class.SupportsForm(form));
			Contract.Requires(!complex || @class is MPrimitiveClass);

			this.@class = @class;
			this.form = (byte)form;
			this.complex = complex;
		}

		public MType(MClass @class, MTypeForm form)
		{
			Contract.Requires(@class != null);
			Contract.Requires(@class.SupportsForm(form));

			this.@class = @class;
			this.form = (byte)form;
			this.complex = false;
		}

		public MType(MClass @class, bool complex)
		{
			Contract.Requires(@class != null);
			Contract.Requires(!complex || @class is MPrimitiveClass);

			this.@class = @class;
			this.form = (byte)@class.DefaultForm;
			this.complex = complex;
		}

		public MType(MClass @class)
		{
			Contract.Requires(@class != null);

			this.@class = @class;
			this.form = (byte)@class.DefaultForm;
			this.complex = false;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the underlying class of this type.
		/// </summary>
		public MClass Class
		{
			get { return @class ?? MPrimitiveClass.Double; }
		}

		/// <summary>
		/// Gets the form of this type.
		/// </summary>
		public MTypeForm Form
		{
			get { return (MTypeForm)form; }
		}

		public bool IsComplex
		{
			get { return complex; }
		}

		public Type RuntimeType
		{
			get { return Class.GetRuntimeType(Form, complex); }
		}
		#endregion

		#region Methods
		public bool Equals(MType other)
		{
			return Class == other.Class && form == other.form && complex == other.complex;
		}

		public override bool Equals(object obj)
		{
			return obj is MType && Equals((MType)obj);
		}

		public override int GetHashCode()
		{
			int hashCode = Class.GetHashCode() ^ ((int)form << 20);
			if (IsComplex) hashCode = ~hashCode;
			return hashCode;
		}
		#endregion

		#region Operators
		public static implicit operator MType(MClass @class)
		{
			return new MType(@class);
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
