using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// An immutable representation of the shape of a matlab array (size along all dimensions).
	/// </summary>
	public struct MArrayShape : IEquatable<MArrayShape>
	{
		#region Fields
		public static readonly MArrayShape Zero = new MArrayShape(0, 0);
		public static readonly MArrayShape Scalar = new MArrayShape(1, 1);

		private readonly int rowCount, columnCount;
		private int[] dimensions;
		#endregion

		#region Constructors
		public MArrayShape(int rowCount, int columnCount)
		{
			this.rowCount = rowCount;
			this.columnCount = columnCount;
			this.dimensions = null;
		}
		#endregion

		#region Properties
		public int RowCount
		{
			get { return rowCount; }
		}

		public int ColumnCount
		{
			get { return columnCount; }
		}

		public int Rank
		{
			get { return dimensions == null ? 2 : dimensions.Length; }
		}

		public int TotalCount
		{
			get
			{
				if (dimensions == null) return rowCount * columnCount;
				throw new NotImplementedException();
			}
		}

		public bool IsScalar
		{
			get { return TotalCount == 1; }
		}
		#endregion

		#region Methods
		public bool Equals(MArrayShape other)
		{
			if (Rank != other.Rank) return false;
			if (Rank > 2) throw new NotImplementedException();

			return rowCount == other.rowCount && columnCount == other.columnCount;
		}

		public override bool Equals(object obj)
		{
			return obj is MArrayShape && Equals((MArrayShape)obj);
		}

		public override int GetHashCode()
		{
			return rowCount ^ (columnCount << 16);
		}

		public int GetSize(int dimensionIndex)
		{
			Contract.Requires(dimensionIndex >= 0 && dimensionIndex < Rank);
			if (dimensionIndex == 0) return rowCount;
			if (dimensionIndex == 1) return columnCount;
			throw new NotImplementedException();
		}

		public static MArrayShape RowVector(int count)
		{
			return new MArrayShape(1, count);
		}

		public static MArrayShape ColumnVector(int count)
		{
			return new MArrayShape(count, 1);
		}
		#endregion

		#region Operators
		public static bool operator==(MArrayShape lhs, MArrayShape rhs)
		{
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(MArrayShape lhs, MArrayShape rhs)
		{
			return !lhs.Equals(rhs);
		}
		#endregion
	}
}
