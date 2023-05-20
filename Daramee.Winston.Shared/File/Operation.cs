using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using static Daramee.Winston.WinstonInterop;

namespace Daramee.Winston.File;

[SupportedOSPlatform("windows")]
public static class Operation
{
    private static IFileOperation? _fileOperation;

    public static bool Begin(bool undoable = true)
    {
        if (_fileOperation != null)
            throw new InvalidOperationException();

        try
        {
            _fileOperation =
                Activator.CreateInstance(
                    Type.GetTypeFromCLSID(new Guid("3ad05575-8857-4850-9277-11b85bdb8e09")) ?? throw new PlatformNotSupportedException()
                ) as IFileOperation;

            _fileOperation?.SetOperationFlags(
                (undoable ? OperationFlag.AllowUndo : OperationFlag.None)
                | OperationFlag.NoUI);
        }
        catch
        {
            return false;
        }

        return _fileOperation != null;
    }

    public static void End(bool doTransaction = true)
    {
        if (_fileOperation == null)
            return;

        if (doTransaction)
        {
            try
            {
                _fileOperation.PerformOperations();
            }
            catch (Exception ex)
            {
                if (!ex.HResult.Equals(unchecked((int) 0x8000FFFF)))
                    throw;
            }
        }

        Marshal.ReleaseComObject(_fileOperation);
        _fileOperation = null;
    }

    public static void Move(string destination, string source, bool overwrite = true)
    {
        if (destination == null)
            throw new ArgumentNullException(nameof(destination));
        
        if (Path.GetFullPath(destination) == Path.GetFullPath(source))
            return;

        if (!overwrite)
            destination = GetNonOverwriteFilename(destination);

        if (_fileOperation == null)
        {
            System.IO.File.Move(source, destination);
        }
        else
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
                string.Equals(destination, source, StringComparison.CurrentCultureIgnoreCase))
            {
                System.IO.File.Move(destination + " ", source);
                source = destination + " ";
            }

            var sourceItem = CreateItem(source);

            long result;
            if (Path.GetDirectoryName(source)
                != Path.GetDirectoryName(destination))
            {
                var destinationPathItem = CreateItem(Path.GetDirectoryName(destination)!);

                result = _fileOperation.MoveItem(sourceItem, destinationPathItem,
                    Path.GetFileName(destination), IntPtr.Zero);

                Marshal.ReleaseComObject(destinationPathItem);
            }
            else
            {
                result = _fileOperation.RenameItem(sourceItem,
                    Path.GetFileName(destination), IntPtr.Zero);
            }

            Marshal.ReleaseComObject(sourceItem);

            AssertHresult(result);
        }
    }

    public static void Copy(string destination, string source, bool overwrite = true)
    {
        if (destination == null)
            throw new ArgumentNullException(nameof(destination));
        
        if (Path.GetFullPath(destination)
            == Path.GetFullPath(source))
            return;

        if (_fileOperation == null)
        {
            System.IO.File.Copy(source, destination, overwrite);
        }
        else
        {
            if (!overwrite)
                destination = GetNonOverwriteFilename(destination);

            var sourceItem = CreateItem(source);
            var destinationPathItem = CreateItem(System.IO.Path.GetDirectoryName(destination)!);

            var result = _fileOperation.CopyItem(sourceItem, destinationPathItem,
                Path.GetFileName(destination), IntPtr.Zero);

            Marshal.ReleaseComObject(destinationPathItem);
            Marshal.ReleaseComObject(sourceItem);

            AssertHresult(result);
        }
    }

    private static string GetNonOverwriteFilename(string filename)
    {
        if (!System.IO.File.Exists(filename)) return filename;

        uint count = 1;
        var path = Path.GetDirectoryName(filename)!;
        var name = Path.GetFileNameWithoutExtension(filename)!;
        var ext = Path.GetExtension(filename)!;

        while (count < 0xfffffffe)
        {
            var newFilename = Path.Combine(path, $"{name} ({count++}){ext}");
            if (!System.IO.File.Exists(newFilename))
                return newFilename;
        }

        throw new IOException();
    }

    public static void Delete(string filename)
    {
        if (_fileOperation != null)
        {
            var fileItem = CreateItem(filename);
            var result = _fileOperation.DeleteItem(fileItem, IntPtr.Zero);
            Marshal.ReleaseComObject(fileItem);
            AssertHresult(result);            
        }
        else
        {
            System.IO.File.Delete(filename);
        }
    }

    private static IShellItem CreateItem(string path)
    {
        SHCreateItemFromParsingName(path, IntPtr.Zero,
            new Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE"),
            out var item);
        return item;
    }

    private static void AssertHresult(long hr)
    {
        if (hr == 0) return;
        throw new IOException("HRESULT exception.", (int) hr);
    }
}