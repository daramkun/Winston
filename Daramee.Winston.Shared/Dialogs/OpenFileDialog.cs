﻿using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using static Daramee.Winston.WinstonInterop;

namespace Daramee.Winston.Dialogs;

[SupportedOSPlatform("windows")]
public class OpenFileDialog : FileDialog
{
    public bool AllowMultiSelection { get; set; }
    public bool NoReadOnlyReturn { get; set; }

    internal override IFileDialog CreateFileDialog()
    {
        return Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7"))) as
            IFileOpenDialog ?? throw new PlatformNotSupportedException();
    }

    internal override FileOpenDialogOptions GetOptions()
    {
        var options = base.GetOptions();
        if (AllowMultiSelection)
            options |= FileOpenDialogOptions.AllowMultiselect;
        if (NoReadOnlyReturn)
            options |= FileOpenDialogOptions.NoReadOnlyReturn;
        return options;
    }

    internal override bool CustomProcess(IFileDialog fileDialog)
    {
        if (!AllowMultiSelection) return false;

        (fileDialog as IFileOpenDialog).GetResults(out var itemArray);
        itemArray.GetCount(out uint resultCount);
        FileNames = new string [resultCount];
        for (uint i = 0; i < resultCount; ++i)
        {
            itemArray.GetItemAt(i, out IShellItem item);
            item.GetDisplayName(ShellItemGetDisplayName.DesktopAbsoluteParsing, out string temp);
            FileNames[i] = temp;
            Marshal.ReleaseComObject(item);
        }

        return true;
    }
}