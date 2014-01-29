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
		internal MClass() { }

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
