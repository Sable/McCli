using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CliKit.IO
{
	public interface ITokenProvider
	{
		MetadataToken Get(MemberInfo member);
		MetadataToken Get(string str);
	}
}
