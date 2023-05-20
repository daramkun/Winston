using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

namespace Daramee.Winston.File;

[SupportedOSPlatform("windows")]
public class StringLogicalComparer : IComparer<string>
{
    private readonly int _modifier = 1;

    public StringLogicalComparer() : this(false)
    {
    }

    public StringLogicalComparer(bool descending)
    {
        if (descending) _modifier = -1;
    }

    public int Compare(string? x, string? y)
    {
        return StrCmpLogicalW(x ?? "", y ?? "") * _modifier;
    }

    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern int StrCmpLogicalW(string psz1, string psz2);
}