using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// Identifies how a value of a MatLab class (real or complex) is stored.
	/// </summary>
	public abstract partial class MStructuralClass
	{
		#region Fields
		public static readonly MStructuralClass Scalar = new ScalarClass();
		public static readonly MStructuralClass Array = new ArrayClass("array", typeof(MArray<>), MClassKinds.PrimitiveMask);
		public static readonly MStructuralClass FullArray = new ArrayClass("full array", typeof(MFullArray<>), MClassKinds.PrimitiveMask);

		private readonly string name;
		private readonly Type containerCliType;
		private readonly bool isArray;
		private readonly bool isMValue;
		private readonly MClassKinds supportedClassKinds;
		private readonly bool supportsComplex;
		#endregion

		#region Constructors
		private MStructuralClass(string name, Type containerCliType, MClassKinds supportedClassKinds, bool supportsComplex)
		{
			Contract.Requires(name != null);

			this.name = name;
			this.containerCliType = containerCliType;
			this.supportedClassKinds = supportedClassKinds & MClassKinds.PrimitiveMask;
			this.supportsComplex = supportsComplex;

			var type = containerCliType;
			while (type != null && type != typeof(object))
			{
				if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(MArray<>))
					isArray = true;
				else if (type == typeof(MValue))
					isMValue = true;

				type = type.BaseType;
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the name of this structural class.
		/// </summary>
		public string Name
		{
			get { return name; }
		}

		/// <summary>
		/// Gets the Common Language Infrastructure container type that represents values with this structural class, if any.
		/// </summary>
		public Type ContainerCliType
		{
			get { return containerCliType; }
		}

		/// <summary>
		/// Gets a value indicating if values with this structural class are <see cref="MValue"/>s.
		/// </summary>
		public bool IsMValue
		{
			get { return isMValue; }
		}

		/// <summary>
		/// Gets a value indicating if values with this structural class are <see cref="MArray{T}"/>s.
		/// </summary>
		public bool IsArray
		{
			get { return isArray; }
		}

		/// <summary>
		/// Gets a mask of class kinds supported by this structural class.
		/// </summary>
		public MClassKinds SupportedClassKinds
		{
			get { return supportedClassKinds; }
		}

		/// <summary>
		/// Gets a value indicating if this structural class supports complex numbers.
		/// </summary>
		public bool SupportsComplex
		{
			get { return supportsComplex; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Determines if a given type can be represented with this structural class.
		/// </summary>
		/// <param name="type">The type to be tested.</param>
		/// <returns>A value indicating if the type can be represented with this structural class.</returns>
		[Pure]
		public bool SupportsType(MType type)
		{
			Contract.Requires(type != null);

			return (!type.IsComplex || SupportsComplex)
				&& (type.Class.Kind & SupportedClassKinds) != 0;
		}

		/// <summary>
		/// Gets the Common Language Infrastructure type for this structural class of a given type.
		/// </summary>
		/// <param name="type">The type to be wrapped in this structural class.</param>
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
		/// Converts a value with this structural class to an array structural class,
		/// (identity if this structural class already represents an array).
		/// </summary>
		/// <param name="value">The value to be converted.</param>
		/// <returns>The corresponding value with an array structural class.</returns>
		public abstract MValue ToArray(object value);

		/// <summary>
		/// Constructs a value with this structural class from a scalar value.
		/// </summary>
		/// <param name="value">The scalar value.</param>
		/// <returns>The corresponding value with this structural class.</returns>
		public abstract object FromScalar(object value);

		public override string ToString()
		{
			return Name;
		}
		#endregion
	}
}
