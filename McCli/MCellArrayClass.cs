using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
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

		public override MClassAttributes SupportedAttributes
		{
			get { return MClassAttributes.None; }
		}
		#endregion

		#region Methods
		public override int GetScalarSizeInBytes(MClassAttributes attributes)
		{
			return 0;
		}
		#endregion
	}
}
