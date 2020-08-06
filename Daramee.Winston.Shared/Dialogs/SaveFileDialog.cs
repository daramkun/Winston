using System;
using static Daramee.Winston.WinstonInterop;

namespace Daramee.Winston.Dialogs
{
	public class SaveFileDialog : FileDialog
	{
		internal override IFileDialog CreateFileDialog ()
		{
			return Activator.CreateInstance ( Type.GetTypeFromCLSID ( new Guid ( "C0B4E2F3-BA21-4773-8DBA-335EC946EB8B" ) ) ) as IFileSaveDialog;
		}
	}
}
