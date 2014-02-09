using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	partial class MPrimitiveForm
	{
		private sealed class ScalarForm : MPrimitiveForm
		{
			internal ScalarForm() : base("scalar", null, MClassKinds.PrimitiveMask, supportsComplex: true) { }

			public override object FromScalar(object value)
			{
				return value;
			}

			public override MValue ToArray(object value)
			{
				throw new NotImplementedException();
			}
		}

		private sealed class ArrayForm : MPrimitiveForm
		{
			internal ArrayForm(string name, Type containerCliType, MClassKinds supportedClassKinds)
				: base(name, containerCliType, supportedClassKinds, supportsComplex: true) { }

			public override object FromScalar(object value)
			{
				throw new NotImplementedException();
			}

			public override MValue ToArray(object value)
			{
				throw new NotImplementedException();
			}
		}
	}
}
