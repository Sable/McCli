using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// Represents a MatLab cell, as is used in a cell array.
	/// </summary>
	public struct MCell
	{
		#region Fields
		private MValue value;
		#endregion

		#region Constructors
		public MCell(MValue value)
		{
			Contract.Requires(value != null);

			this.value = value;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the value in this MatLab cell.
		/// </summary>
		public MValue Value
		{
			get
			{
				Initialize();
				return value;
			}
		}
		#endregion

		#region Methods
		public MCell Initialize()
		{
			if (value == null) value = MFullArray<double>.CreateEmpty();
			return this;
		}

		public MCell DeepClone()
		{
			Initialize();
			return new MCell(value.DeepClone());
		}
		#endregion

		#region Operators
		public static implicit operator MCell(MValue value)
		{
			return new MCell(value);
		}
		#endregion
	}
}
