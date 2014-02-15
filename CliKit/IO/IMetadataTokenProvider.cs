using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CliKit.IO
{
	public interface IMetadataTokenProvider
	{
		MetadataToken GetMemberToken(MemberInfo member);
		MetadataToken GetStringToken(string value);
		// TODO: Add signatures and other stuff from DynamicILInfo
	}
}
