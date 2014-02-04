using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// Further abstraction of a MatLab class that differentiates between real and complex numerical types.
	/// </summary>
	public abstract class MType
	{
		#region Constructors
		internal MType() { }
		#endregion

		#region Properties
		/// <summary>
		/// Gets the class underlying this type.
		/// </summary>
		public MClass Class
		{
			get { return GetClass(); }
		}

		/// <summary>
		/// Gets the Common Language Infrastructure type that represents this type.
		/// </summary>
		public abstract Type CliType { get; }
		
		/// <summary>
		/// Gets the name of this type.
		/// </summary>
		public abstract string Name { get; }

		/// <summary>
		/// Gets a value indicating if this type is of a primitive class.
		/// </summary>
		public bool IsPrimitive
		{
			get { return (Class.Kind & MClassKinds.PrimitiveMask) != 0; }
		}

		/// <summary>
		/// Gets a value indicating if this is a complex numeric type.
		/// </summary>
		public bool IsComplex
		{
			get { return this is MComplexType; }
		}

		/// <summary>
		/// Gets the size in bytes of values of this type.
		/// Returns zero if the value is undeterminate.
		/// </summary>
		public abstract int FixedSizeInBytes { get; }
		#endregion

		#region Methods
		public override sealed string ToString()
		{
			return Name;
		}

		protected abstract MClass GetClass();

		public static MType FromCliType(Type type)
		{
			Contract.Requires(type != null);
			return MTypeLookup.ByCliType(type);
		}
		#endregion
	}
}
