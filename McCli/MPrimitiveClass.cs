using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// Represents primitive MatLab classes: numerical (floating-point and integral), character and logical.
	/// </summary>
	public sealed class MPrimitiveClass : MClass
	{
		#region Fields;
		private readonly Type cliType;
		private readonly string name;
		#endregion

		#region Constructors
		internal MPrimitiveClass(Type cliType, string name)
		{
			Contract.Requires(cliType != null);

			this.cliType = cliType;
			this.name = name;
		}
		#endregion

		#region Properties
		public override string Name
		{
			get { return name; }
		}

		public override Type CliType
		{
			get { return cliType; }
		}

		public bool IsNumerical
		{
			get { return cliType != typeof(char) && cliType != typeof(bool); }
		}

		public bool IsFloat
		{
			get { return cliType == typeof(float) || cliType == typeof(double); }
		}

		public bool IsInteger
		{
			get { return name[0] == 'i' || name[0] == 'u'; }
		}

		public bool IsSignedInteger
		{
			get { return name[0] == 'i'; }
		}

		public bool IsUnsignedInteger
		{
			get { return name[0] == 'u'; }
		}

		public override MTypeLayers ValidTypeLayers
		{
			get
			{
				var layers = MTypeLayers.Array | MTypeLayers.DenseArray | MTypeLayers.SparseMatrix;
				if (IsNumerical) layers |= MTypeLayers.Complex;
				return layers;
			}
		}

		public override MTypeLayers DefaultTypeLayers
		{
			get { return MTypeLayers.DenseArray; }
		}
		#endregion

		#region Methods
		public MType AsScalarType()
		{
			return new MType(this, MTypeLayers.None);
		}

		public MType AsComplexType()
		{
			Contract.Requires((ValidTypeLayers & MTypeLayers.Complex) != 0);
			return new MType(this, MTypeLayers.Complex);
		}

		public MType AsArrayType()
		{
			return new MType(this, MTypeLayers.Array);
		}

		public MType AsDenseArrayType()
		{
			return new MType(this, MTypeLayers.DenseArray);
		}

		public override int GetScalarSizeInBytes(MTypeLayers layers)
		{
			// Marshal.SizeOf returns 1 for 'char' and 'bool', which is what we want.
			int size = Marshal.SizeOf(cliType);
			if ((layers & MTypeLayers.Complex) != 0) size *= 2;
			return size;
		}
		#endregion
	}
}
