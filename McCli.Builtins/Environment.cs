using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Builtins
{
	/// <summary>
	/// Implements MatLab builtins which relate to the execution environment.
	/// </summary>
	public static class Environment
	{
		public static bool ispc()
		{
			var platform = System.Environment.OSVersion.Platform;
			return platform == PlatformID.Win32NT
				|| platform == PlatformID.Win32Windows
				|| platform == PlatformID.Win32S;
		}

		public static bool isunix()
		{
			return System.Environment.OSVersion.Platform == PlatformID.Unix;
		}

		public static bool ismac()
		{
			return System.Environment.OSVersion.Platform == PlatformID.MacOSX;
		}

		public static bool isstudent()
		{
			return false; // !
		}
	}
}
