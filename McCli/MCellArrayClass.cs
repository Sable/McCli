using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// Represents the MatLab class of cell arrays.
	/// </summary>
	public sealed class MCellArrayClass : MClass
	{
		#region Constructors
		internal MCellArrayClass() { }
		#endregion

		#region Properties
		public override string Name
		{
			get { return "cell"; }
		}

		public override Type CliType
		{
			get { return typeof(MCellArray); }
		}

		public override MTypeLayers ValidTypeLayers
		{
			get { return MTypeLayers.None; }
		}
		#endregion

		#region Methods
		public override int GetScalarSizeInBytes(MTypeLayers layers)
		{
			return 0;
		}
		#endregion
	}
}
