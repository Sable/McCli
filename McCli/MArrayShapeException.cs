using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	public sealed class MArrayShapeException : ArgumentException
	{
		public MArrayShapeException() : base("Invalid array shape.") { }
		public MArrayShapeException(string message) : base(message) { }

		public static MArrayShapeException CreateFormatted(string format, params object[] args)
		{
			return new MArrayShapeException(string.Format(CultureInfo.InvariantCulture, format, args));
		}
	}
}
