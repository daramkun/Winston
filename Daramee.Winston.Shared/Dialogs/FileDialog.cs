using System;
using System.Runtime.InteropServices;
using static Daramee.Winston.WinstonInterop;

namespace Daramee.Winston.Dialogs
{
	public abstract class FileDialog : IDisposable
	{
		public virtual string Title { get; set; }
		public virtual string FileName { get; set; }
		public virtual string [] FileNames { get; protected set; }
		public virtual string InitialDirectory { get; set; }
		public virtual string Filter { get; set; }

		public virtual bool ShowHiddenItems { get; set; }

		protected FileDialog ()
		{
			if ( !Utility.IsAvailablePlatform () )
				throw new PlatformNotSupportedException ();
		}

		~FileDialog ()
		{
			Dispose ( false );
		}

		public void Dispose ()
		{
			Dispose ( true );
			GC.SuppressFinalize ( this );
		}

		public virtual void Dispose ( bool disposing )
		{

		}

		internal abstract IFileDialog CreateFileDialog ();
		internal virtual FileOpenDialogOptions GetOptions ()
		{
			FileOpenDialogOptions options = FileOpenDialogOptions.NoTestFileCreate
				| FileOpenDialogOptions.ForceFileSystem;
			if ( ShowHiddenItems )
				options |= FileOpenDialogOptions.ForceShowHidden;
			return options;
		}
		internal virtual bool CustomProcess ( IFileDialog dialog )
		{
			return false;
		}
		
		public bool? ShowDialog ()
		{
			return ShowDialog ( IntPtr.Zero );
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

		public bool? ShowDialog ( IntPtr parent )
		{
			IFileDialog fileDialog = CreateFileDialog ();
			fileDialog.SetTitle ( Title ?? "" );
			var option = GetOptions ();
			fileDialog.SetOptions ( option );
			var specs = GetFilter ();
			if ( specs != null )
				fileDialog.SetFileTypes ( ( uint ) specs.Length, specs );
			else if ( !option.HasFlag ( FileOpenDialogOptions.PickFolders ) )
				fileDialog.SetFileTypes ( 0, new CommonDialogFilterSpecification [ 0 ] );
			fileDialog.SetFileName ( FileName ?? "" );
			if ( !string.IsNullOrEmpty ( InitialDirectory ) )
			{
				SHCreateItemFromParsingName ( InitialDirectory, IntPtr.Zero,
				new Guid ( "43826D1E-E718-42EE-BC55-A1E261C37BFE" ),
				out IShellItem initialDirectory );
				fileDialog.SetFolder ( initialDirectory );
				Marshal.ReleaseComObject ( initialDirectory );
			}
			int result = fileDialog.Show ( parent );
			if ( result == HResultFromWin32 ( 1223 ) )
			{
				Marshal.ReleaseComObject ( fileDialog );
				return false;
			}
			else if ( result != 0 )
			{
				Marshal.ReleaseComObject ( fileDialog );
				throw new COMException ( "Unknown COM Exception.", result );
			}

			// TODO
			if ( !CustomProcess ( fileDialog ) )
			{
				fileDialog.GetResult ( out IShellItem item );
				item.GetDisplayName ( ShellItemGetDisplayName.DesktopAbsoluteParsing, out string temp );
				FileName = temp;
				Marshal.ReleaseComObject ( item );
			}
			Marshal.ReleaseComObject ( fileDialog );

			return true;
		}

		private static int HResultFromWin32 ( int win32ErrorCode )
		{
			const uint FACILITY_WIN32 = 7;
			if ( win32ErrorCode > 0 )
			{
				win32ErrorCode =
					( int ) ( ( win32ErrorCode & 0x0000FFFF ) | ( FACILITY_WIN32 << 16 ) | 0x80000000 );
			}
			return win32ErrorCode;
		}

		private CommonDialogFilterSpecification [] GetFilter ()
		{
			if ( string.IsNullOrEmpty ( Filter ) )
				return null;
			else
			{
				string [] items = Filter.Split ( '|' );
				CommonDialogFilterSpecification [] specs = new CommonDialogFilterSpecification [ items.Length / 2 ];
				for ( int i = 0; i < specs.Length; ++i )
				{
					specs [ i ].Name = items [ i * 2 ];
					specs [ i ].Specification = items [ i * 2 + 1 ];
				}
				return specs;
			}
		}
	}
}
