using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// Represents the MatLab class of function handles.
	/// </summary>
	public sealed class MFunctionHandleClass : MClass
	{
		#region Constructors
		internal MFunctionHandleClass() { }
		#endregion

		#region Properties
		public override MClassKinds Kind
		{
			get { return MClassKinds.FunctionHandle; }
		}

		public override string Name
		{
			get { return "function_handle"; }
		}

		public override Type CliType
		{
			get { throw new NotImplementedException(); }
		}

		public override int FixedSizeInBytes
		{
			get { return 0; }
		}

		public override MTypeLayers ValidTypeLayers
		{
			get { return MTypeLayers.None; }
		}
		#endregion
	}
}
