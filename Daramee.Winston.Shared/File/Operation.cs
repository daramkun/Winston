using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using static Daramee.Winston.WinstonInterop;

namespace Daramee.Winston.File
{
    public static class Operation
    {
		static IFileOperation fileOperation;

		public static bool Begin ( bool undoable = true )
		{
			if ( fileOperation != null )
				throw new InvalidOperationException ();

			try
			{
				fileOperation = Activator.CreateInstance ( Type.GetTypeFromCLSID ( new Guid ( "3ad05575-8857-4850-9277-11b85bdb8e09" ) ) ) as IFileOperation;
				fileOperation?.SetOperationFlags (
					( undoable ? OperationFlag.AllowUndo : OperationFlag.None )
					| OperationFlag.NoUI | OperationFlag.FilesOnly );
			}
			catch { return false; }

			return fileOperation != null;
		}

		public static void End ( bool doTransaction = true )
		{
			if ( fileOperation != null )
			{
				if ( doTransaction )
					fileOperation.PerformOperations ();
				Marshal.ReleaseComObject ( fileOperation );
				fileOperation = null;
			}
		}

		public static void Move ( string destination, string source, bool overwrite = true )
		{
			if ( System.IO.Path.GetFullPath ( destination )
				== System.IO.Path.GetFullPath ( source ) )
				return;

			if ( overwrite && System.IO.File.Exists ( destination ) )
				System.IO.File.Delete ( destination );
			if ( fileOperation == null )
			{
				System.IO.File.Move ( source, destination );
			}
			else
			{
				long result = 0;

				IShellItem sourceItem = CreateItem ( source );

				if ( System.IO.Path.GetDirectoryName ( source )
					!= System.IO.Path.GetDirectoryName ( destination ) )
				{
					IShellItem destinationPathItem = CreateItem ( System.IO.Path.GetDirectoryName ( destination ) );
					
					result = fileOperation.MoveItem ( sourceItem, destinationPathItem,
						System.IO.Path.GetFileName ( destination ), IntPtr.Zero );

					Marshal.ReleaseComObject ( destinationPathItem );
				}
				else
				{
					result = fileOperation.RenameItem ( sourceItem,
						System.IO.Path.GetFileName ( destination ), IntPtr.Zero );
				}

				Marshal.ReleaseComObject ( sourceItem );

				AssertHRESULT ( result );
			}
		}

		public static void Copy ( string destination, string source, bool overwrite = true )
		{
			if ( System.IO.Path.GetFullPath ( destination )
				== System.IO.Path.GetFullPath ( source ) )
				return;

			if ( fileOperation == null )
			{
				System.IO.File.Copy ( source, destination, overwrite );
			}
			else
			{
				if ( overwrite && System.IO.File.Exists ( destination ) )
					System.IO.File.Delete ( destination );

				IShellItem sourceItem = CreateItem ( source );
				IShellItem destinationPathItem = CreateItem ( System.IO.Path.GetDirectoryName ( destination ) );

				long result = fileOperation.CopyItem ( sourceItem, destinationPathItem,
					System.IO.Path.GetFileName ( destination ), IntPtr.Zero );

				Marshal.ReleaseComObject ( destinationPathItem );
				Marshal.ReleaseComObject ( sourceItem );

				AssertHRESULT ( result );
			}
		}

		public static void Delete ( string filename )
		{
			IShellItem fileItem = CreateItem ( filename );
			long result = fileOperation.DeleteItem ( fileItem, IntPtr.Zero );
			Marshal.ReleaseComObject ( fileItem );
			AssertHRESULT ( result );
		}

		private static IShellItem CreateItem ( string path )
		{
			SHCreateItemFromParsingName ( path, IntPtr.Zero,
				new Guid ( "43826D1E-E718-42EE-BC55-A1E261C37BFE" ),
				out IShellItem item );
			return item;
		}

		private static void AssertHRESULT ( long hr )
		{
			if ( hr == 0 ) return;
			throw new IOException ( "HRESULT exception.", ( int ) hr );
		}
	}
}
