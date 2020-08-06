using System;
using System.Runtime.InteropServices;
using static Daramee.Winston.WinstonInterop;

namespace Daramee.Winston.Dialogs
{
	public class OpenFileDialog : FileDialog
	{
		public bool AllowMultiSelection { get; set; }
		public bool NoReadOnlyReturn { get; set; }

		internal override IFileDialog CreateFileDialog ()
		{
			return Activator.CreateInstance ( Type.GetTypeFromCLSID ( new Guid ( "DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7" ) ) ) as IFileOpenDialog;
		}

		internal override FileOpenDialogOptions GetOptions ()
		{
			FileOpenDialogOptions options = base.GetOptions ();
			if ( AllowMultiSelection )
				options |= FileOpenDialogOptions.AllowMultiselect;
			if ( NoReadOnlyReturn )
				options |= FileOpenDialogOptions.NoReadOnlyReturn;
			return options;
		}

		internal override bool CustomProcess ( IFileDialog fileDialog )
		{
			if ( !AllowMultiSelection ) return false;

			( fileDialog as IFileOpenDialog ).GetResults ( out IShellItemArray itemArray );
			itemArray.GetCount ( out uint resultCount );
			FileNames = new string [ resultCount ];
			for ( uint i = 0; i < resultCount; ++i )
			{
				itemArray.GetItemAt ( i, out IShellItem item );
				item.GetDisplayName ( ShellItemGetDisplayName.DesktopAbsoluteParsing, out string temp );
				FileNames [ i ] = temp;
				Marshal.ReleaseComObject ( item );
			}

			return true;
		}
	}
}
