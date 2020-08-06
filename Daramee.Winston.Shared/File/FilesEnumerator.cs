using System;
using System.Collections.Generic;
using System.IO;
using static Daramee.Winston.WinstonInterop;

namespace Daramee.Winston.File
{
    public static class FilesEnumerator
    {
		public static IEnumerable<string> EnumerateFiles ( string path, string pattern, bool topDirectoryOnly = true )
		{
			bool allFinding = false;
			IntPtr hFind = FindFirstFile ( Path.Combine ( path, pattern ), out WIN32_FIND_DATA findData );
			if ( hFind == new IntPtr ( -1 ) )
			{
				hFind = FindFirstFile ( Path.Combine ( path, "*" ), out findData );
				if ( hFind == new IntPtr ( -1 ) )
					yield break;
				allFinding = true;
			}

			do
			{
				if ( findData.dwFileAttributes.HasFlag ( FileAttributes.Directory ) )
				{
					if ( findData.cFileName == "." || findData.cFileName == ".." )
						continue;
					if ( topDirectoryOnly )
						continue;
					foreach ( var innerfile in EnumerateFiles ( Path.Combine ( path, findData.cFileName ), pattern, false ) )
						yield return innerfile;
				}
				string ret = Path.Combine ( path, findData.cFileName );
				if ( ret != path )
				{
					if ( ( allFinding && PathMatchSpec ( ret, pattern ) ) || !allFinding )
						yield return ret;
				}
			} while ( FindNextFile ( hFind, out findData ) );

			FindClose ( hFind );
		}
    }
}
