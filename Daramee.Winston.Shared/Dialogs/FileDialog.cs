using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using static Daramee.Winston.WinstonInterop;

namespace Daramee.Winston.Dialogs;

[SupportedOSPlatform("windows")]
public abstract class FileDialog : IDisposable
{
    public virtual string Title { get; set; } = string.Empty;
    public virtual string FileName { get; set; } = string.Empty;
    public virtual string[]? FileNames { get; protected set; }
    public virtual string? InitialDirectory { get; set; } = null;
    public virtual string? Filter { get; set; } = null;

    public virtual bool ShowHiddenItems { get; set; }

    protected FileDialog()
    {
        if (!Utility.IsAvailablePlatform())
            throw new PlatformNotSupportedException();
    }

    ~FileDialog()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public virtual void Dispose(bool disposing)
    {
    }

    internal abstract IFileDialog CreateFileDialog();

    internal virtual FileOpenDialogOptions GetOptions()
    {
        FileOpenDialogOptions options = FileOpenDialogOptions.NoTestFileCreate
                                        | FileOpenDialogOptions.ForceFileSystem;
        if (ShowHiddenItems)
            options |= FileOpenDialogOptions.ForceShowHidden;
        return options;
    }

    internal virtual bool CustomProcess(IFileDialog dialog)
    {
        return false;
    }

    public bool? ShowDialog()
    {
        return ShowDialog(IntPtr.Zero);
    }

#if WINFORMS
		public bool? ShowDialog ( System.Windows.Forms.Form window )
		{
			return ShowDialog ( window.Handle );
		}
#endif
#if WPF
		public bool? ShowDialog ( System.Windows.Window window )
		{
			return ShowDialog ( new System.Windows.Interop.WindowInteropHelper ( window ).Handle );
		}
#endif

    public bool? ShowDialog(IntPtr parent)
    {
        var fileDialog = CreateFileDialog();
        fileDialog.SetTitle(Title ?? "");
        var option = GetOptions();
        fileDialog.SetOptions(option);
        var specs = GetFilter();
        if (specs != null)
            fileDialog.SetFileTypes((uint) specs.Length, specs);
        else if (!option.HasFlag(FileOpenDialogOptions.PickFolders))
            fileDialog.SetFileTypes(0, Array.Empty<CommonDialogFilterSpecification>());
        fileDialog.SetFileName(FileName ?? "");
        if (!string.IsNullOrEmpty(InitialDirectory))
        {
            SHCreateItemFromParsingName(InitialDirectory, IntPtr.Zero,
                new Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE"),
                out IShellItem initialDirectory);
            fileDialog.SetFolder(initialDirectory);
            Marshal.ReleaseComObject(initialDirectory);
        }

        var result = fileDialog.Show(parent);
        if (result == HResultFromWin32(1223))
        {
            Marshal.ReleaseComObject(fileDialog);
            return false;
        }
        if (result != 0)
        {
            Marshal.ReleaseComObject(fileDialog);
            throw new COMException("Unknown COM Exception.", result);
        }

        // TODO
        if (!CustomProcess(fileDialog))
        {
            fileDialog.GetResult(out var item);
            item.GetDisplayName(ShellItemGetDisplayName.DesktopAbsoluteParsing, out var temp);
            FileName = temp;
            Marshal.ReleaseComObject(item);
        }

        Marshal.ReleaseComObject(fileDialog);

        return true;
    }

    private static int HResultFromWin32(int win32ErrorCode)
    {
        const uint facilityWin32 = 7;
        if (win32ErrorCode > 0)
        {
            win32ErrorCode =
                (int) ((win32ErrorCode & 0x0000FFFF) | (facilityWin32 << 16) | 0x80000000);
        }

        return win32ErrorCode;
    }

    private CommonDialogFilterSpecification[]? GetFilter()
    {
        if (string.IsNullOrEmpty(Filter))
            return null;
        else
        {
            string[] items = Filter.Split('|');
            var specs = new CommonDialogFilterSpecification [items.Length / 2];
            for (var i = 0; i < specs.Length; ++i)
            {
                specs[i].Name = items[i * 2];
                specs[i].Specification = items[i * 2 + 1];
            }

            return specs;
        }
    }
}