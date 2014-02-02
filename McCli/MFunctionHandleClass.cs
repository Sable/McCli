﻿using System;
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
		public override string Name
		{
			get { return "function_handle"; }
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
