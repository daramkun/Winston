using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static Daramee.Winston.WinstonInterop;

namespace Daramee.Winston.File
{
    public static class FilesEnumerator
    {
		public static IEnumerable<string> EnumerateFiles ( string path, string pattern, bool topDirectoryOnly = true )
		{
			IntPtr hFind = FindFirstFile ( Path.Combine ( path, pattern ), out WIN32_FIND_DATA findData );
			if ( hFind == new IntPtr ( -1 ) )
				yield break;

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
				yield return Path.Combine ( path, findData.cFileName );
			} while ( FindNextFile ( hFind, out findData ) );

			FindClose ( hFind );
		}
    }
}
