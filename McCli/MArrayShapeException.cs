using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	public sealed class MArrayShapeException : ArgumentException
	{
		public MArrayShapeException() : base("Invalid array shape.") { }
		public MArrayShapeException(string message) : base(message) { }
	}
}
