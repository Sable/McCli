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
	/// </summary>
	public abstract class MClass : MType
	{
		internal MClass() { }

		#region Properties
		public override MClass Class
		{
			get { return this; }
		}

		/// <summary>
		/// Gets the name of this class.
		/// </summary>
		public abstract string Name { get; }
		#endregion

		#region Methods
		public override bool Equals(MType other)
		{
			return ReferenceEquals(other, this);
		}

		public override int GetHashCode()
		{
			return RuntimeHelpers.GetHashCode(this);
		}
		#endregion
	}
}
