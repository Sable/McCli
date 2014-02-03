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
	public struct MType : IEquatable<MType>
	{
		#region Fields
		public static readonly MValue Any = default(MValue);
		
		private readonly MClass @class;
		private readonly MTypeLayers layers;
		#endregion

		#region Constructors
		public MType(MClass @class, MTypeLayers layers)
		{
			Contract.Requires(@class == null || ((@class.ValidTypeLayers & layers) == layers));

			this.@class = @class;
			this.layers = layers;
		}
		#endregion

		#region Properties
		public MClass Class
		{
			get { return @class; }
		}

		public bool IsAny
		{
			get { return @class == null; }
		}

		public MTypeLayers Layers
		{
			get { return layers; }
		}

		public bool IsComplex
		{
			get { return (layers & MTypeLayers.Complex) == MTypeLayers.Complex; }
		}

		public bool IsArray
		{
			get { return (layers & MTypeLayers.Array) == MTypeLayers.Array; }
		}

		public bool IsDenseArray
		{
			get { return (layers & MTypeLayers.DenseArray) == MTypeLayers.DenseArray; }
		}

		public bool IsSparseMatrix
		{
			get { return (layers & MTypeLayers.SparseMatrix) == MTypeLayers.SparseMatrix; }
		}

		public bool IsScalar
		{
			get { return @class != null && @class.CliType.IsValueType && (layers & MTypeLayers.Array) == 0; }
		}

		public bool IsBoxedAsMValue
		{
			get { return !IsScalar; }
		}

		public bool IsConcrete
		{
			get
			{
				return @class != null
					&& ((layers & MTypeLayers.Array) == 0
						|| (layers & (MTypeLayers.DenseArrayFlag | MTypeLayers.SparseMatrixFlag)) != 0);
			}
		}

		public Type CliType
		{
			get
			{
				if (@class == null) return typeof(MValue);

				var cliType = @class.CliType;
				if (IsComplex) cliType = typeof(MComplex<>).MakeGenericType(cliType);
				if (IsDenseArray) cliType = typeof(MDenseArray<>).MakeGenericType(cliType);
				else if (IsArray) cliType = typeof(MArray<>).MakeGenericType(cliType);
				return cliType;
			}
		}
		#endregion

		#region Methods
		public bool Equals(MType other)
		{
			return @class == other.@class && layers == other.layers;
		}

		public override bool Equals(object obj)
		{
			return obj is MType && Equals((MType)obj);
		}

		public override int GetHashCode()
		{
			return (@class == null ? 0 : @class.GetHashCode()) ^ ((int)layers << 20);
		}

		public override string ToString()
		{
			if (@class == null) return "any";
			if (layers == MTypeLayers.None) return @class.Name;
			return string.Format(CultureInfo.InvariantCulture, "{0} [{1}]", @class.Name, layers);
		}

		public static MType FromCliType(Type type)
		{
			Contract.Requires(type != null);

			var layers = MTypeLayers.None;
			while (type.IsGenericType)
			{
				var genericTypeDefinition = type.GetGenericTypeDefinition();
				if (genericTypeDefinition == typeof(MDenseArray<>))
					layers |= MTypeLayers.DenseArray;
				else if (genericTypeDefinition == typeof(MComplex<>))
					layers |= MTypeLayers.Complex;
				else if (genericTypeDefinition == typeof(MArray<>))
					layers |= MTypeLayers.Array;
				else
					throw new ArgumentException("Not a valid MatLab type.", "type");

				type = type.GetGenericArguments()[0];
			}

			var @class = MClass.FromCliType(type);
			if (@class == null && !typeof(MValue).IsAssignableFrom(type))
				throw new ArgumentException("Not a valid MatLab type.", "type");

			return new MType(@class, layers);
		}
		#endregion

		#region Operators
		public static bool operator==(MType lhs, MType rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(MType lhs, MType rhs)
		{
			return !lhs.Equals(rhs);
		}
		#endregion
	}
}
