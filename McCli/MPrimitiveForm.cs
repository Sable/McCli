using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// Identifies a representation of a primitive type (including complex numbers).
	/// </summary>
	public abstract partial class MPrimitiveForm
	{
		#region Fields
		public static readonly MPrimitiveForm Scalar = new ScalarForm();
		public static readonly MPrimitiveForm Array = new ArrayForm("array", typeof(MArray<>), MClassKinds.PrimitiveMask);
		public static readonly MPrimitiveForm FullArray = new ArrayForm("full array", typeof(MFullArray<>), MClassKinds.PrimitiveMask);

		private readonly string name;
		private readonly Type containerCliType;
		private readonly bool isArray;
		private readonly MClassKinds supportedClassKinds;
		private readonly bool supportsComplex;
		#endregion

		#region Constructors
		private MPrimitiveForm(string name, Type containerCliType, MClassKinds supportedClassKinds, bool supportsComplex)
		{
			Contract.Requires(name != null);

			this.name = name;
			this.containerCliType = containerCliType;
			this.supportedClassKinds = supportedClassKinds & MClassKinds.PrimitiveMask;
			this.supportsComplex = supportsComplex;

			var type = containerCliType;
			while (type != null && type != typeof(object))
			{
				if (type == typeof(MArray)) isArray = true;
				type = type.BaseType;
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the name of this form.
		/// </summary>
		public string Name
		{
			get { return name; }
		}

		/// <summary>
		/// Gets the Common Language Infrastructure container type that represents values in this form, if any.
		/// </summary>
		public Type ContainerCliType
		{
			get { return containerCliType; }
		}

		/// <summary>
		/// Gets a value indicating if this form is an array form.
		/// </summary>
		public bool IsArray
		{
			get { return isArray; }
		}

		/// <summary>
		/// Gets a mask of class kinds supported by this form.
		/// </summary>
		public MClassKinds SupportedClassKinds
		{
			get { return supportedClassKinds; }
		}

		/// <summary>
		/// Gets a value indicating if this form supports complex numbers.
		/// </summary>
		public bool SupportsComplex
		{
			get { return supportsComplex; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Determines if a given type can be represented in this form.
		/// </summary>
		/// <param name="type">The type to be tested.</param>
		/// <returns>A value indicating if the type can be represented in this form.</returns>
		[Pure]
		public bool SupportsType(MType type)
		{
			Contract.Requires(type != null);

			return (!type.IsComplex || SupportsComplex)
				&& (type.Class.Kind & SupportedClassKinds) != 0;
		}

		/// <summary>
		/// Gets the Common Language Infrastructure type for this form of a given type.
		/// </summary>
		/// <param name="type">The type to be wrapped in this form.</param>
		/// <returns>The resulting CLI type.</returns>
		[Pure]
		public Type GetCliType(MType type)
		{
			Contract.Requires(type != null && SupportsType(type));

			var scalarCliType = type.CliType;
			var containerCliType = ContainerCliType;
			if (containerCliType == null) return scalarCliType;
			if (!containerCliType.IsGenericTypeDefinition) return containerCliType;
			return containerCliType.MakeGenericType(scalarCliType);
		}

		/// <summary>
		/// Converts a value in this form to a array form, (identity if the value is already an array).
		/// </summary>
		/// <param name="value">The value to be converted.</param>
		/// <returns>The corresponding value in array form.</returns>
		public abstract MArray ToArray(object value);

		/// <summary>
		/// Constructs a value in this from from a scalar value.
		/// </summary>
		/// <param name="value">The scalar value.</param>
		/// <returns>The corresponding value in this form.</returns>
		public abstract object FromScalar(object value);

		public override string ToString()
		{
			return Name;
		}
		#endregion
	}
}
