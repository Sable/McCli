using McCli.Compilation.IR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	[TestClass]
	public sealed class TestCompilation
	{
		[TestMethod]
		public void TestIdentity()
		{
			var input = new Name("input");
			var output = new Name("output");
			var name = new Name("identity");

			var function = new Function(name,
				ImmutableArray.Create(input),
				ImmutableArray.Create(output),
				ImmutableArray.Create<Statement>(
					new Copy(input, output))
				);

		}
	}
}
