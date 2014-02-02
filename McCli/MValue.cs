using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// Base class for boxed MatLab variable values.
	/// </summary>
	public abstract class MValue
	{
		#region Fields
		#endregion

		#region Constructors
		internal MValue() { }
		#endregion

		#region Properties
		/// <summary>
		/// Gets the class of this value.
		/// </summary>
		public abstract MClass Class { get; }

		public abstract MClassAttributes ClassAttributes { get; }
		#endregion

		#region Methods
		/// <summary>
		/// Obtains a deep clone of this value.
		/// </summary>
		/// <returns>A deep clone of this value, or the value itself if it is immutable.</returns>
		public MValue DeepClone()
		{
			return DoDeepClone();
		}

		protected abstract MValue DoDeepClone();
		#endregion
	}
}
