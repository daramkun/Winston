using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Daramee.Winston
{
	public static class Utility
	{
		public static bool IsAvailablePlatform ()
		{
#if NET40 || NET45 || NET46 || NET47 || WPF
			if ( Environment.OSVersion.Platform == PlatformID.Win32NT
				&& Environment.OSVersion.Version >= new Version ( 6, 0 ) )
				return true;
			return false;
#elif NETSTANDARD2_0
			return RuntimeInformation.IsOSPlatform ( OSPlatform.Windows );
#endif
		}
	}
}
