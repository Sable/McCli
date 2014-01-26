using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	public abstract class MValue
	{
		internal MValue() { }

		public abstract MType MType { get; }
	}
}
