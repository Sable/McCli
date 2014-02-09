using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// Represents the MatLab class of cells (and cell arrays).
	/// </summary>
	public sealed class MCellClass : MClass
	{
		#region Constructors
		internal MCellClass() { }
		#endregion

		#region Properties
		public override MClassKinds Kind
		{
			get { return MClassKinds.Cell; }
		}

		public override string Name
		{
			get { return "cell"; }
		}

		public override Type CliType
		{
			get { return typeof(MCell); }
		}

		public override int FixedSizeInBytes
		{
			get { return 0; }
		}
		#endregion
	}
}
