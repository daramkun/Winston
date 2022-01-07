using System.Runtime.InteropServices;

namespace Daramee.Winston
{
    public static class Utility
    {
        public static bool IsAvailablePlatform()
        {
#if NETSTANDARD2_0_OR_GREATER
			return RuntimeInformation.IsOSPlatform (OSPlatform.Windows);
#elif NET40 || NET45 || NET46 || NET47
			if ( Environment.OSVersion.Platform == PlatformID.Win32NT
				&& Environment.OSVersion.Version >= new Version ( 6, 0 ) )
				return true;
			return false;
#endif
        }
    }
}