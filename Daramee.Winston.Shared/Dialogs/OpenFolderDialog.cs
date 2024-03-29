﻿using System;
using System.Runtime.Versioning;
using static Daramee.Winston.WinstonInterop;

namespace Daramee.Winston.Dialogs;

[SupportedOSPlatform("windows")]
public class OpenFolderDialog : OpenFileDialog
{
    public override string? Filter
    {
        get => null;
        set => throw new InvalidOperationException("Open Folder Dialog cannot set Filter.");
    }

    internal override FileOpenDialogOptions GetOptions()
    {
        return base.GetOptions() | FileOpenDialogOptions.PickFolders;
    }
}