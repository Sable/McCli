using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	public sealed class MFunctionHandleClass : MClass
	{
		#region Constructors
		internal MFunctionHandleClass() { }
		#endregion

		#region Properties
		public override Type BasicScalarType
		{
			get { return typeof(Delegate); }
		}

		public override MTypeForm DefaultForm
		{
			get { return MTypeForm.Scalar; }
		}

		public override string Name
		{
			get { return "function_handle"; }
		}

		public override int FixedSizeInBytes
		{
			get { return 0; }
		}
		#endregion

		#region Methods
		public override bool SupportsForm(MTypeForm form)
		{
			return form == MTypeForm.Scalar;
		}
		#endregion
	}
}
