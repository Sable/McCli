using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	[Flags]
	public enum MClassAttributes
	{
		None = 0,
		Complex = 1 << 0,
		SparseMatrix = 1 << 1,
	}
}
