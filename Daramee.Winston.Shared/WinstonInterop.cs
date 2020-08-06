using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Daramee.Winston
{
    internal static class WinstonInterop
	{
		[Flags]
		public enum OperationFlag : uint
		{
			None = 0x0000,
			MultiDestinationFiles = 0x0001,
			ConfirmMouse = 0x0002,
			Silent = 0x0004,
			RenameOnCollision = 0x0008,
			NoConfirmation = 0x0010,
			WantMappingHandle = 0x0020,
			AllowUndo = 0x0040,
			FilesOnly = 0x0080,
			SimpleProgress = 0x0100,
			NoConfirmMakeDirectory = 0x0200,
			NoErrorUI = 0x0400,
			NoCopySecurityAttributes = 0x0800,
			NoRecursion = 0x1000,
			NoConnectedElements = 0x2000,
			WantNukeWarning = 0x4000,
			NoRecurseReparse = 0x8000,
			NoUI = ( Silent | NoConfirmation | NoErrorUI | NoConfirmMakeDirectory ),
		}

		public enum ShellItemGetDisplayName : uint
		{
			NormalDisplay = 0x00000000,
			ParentRelativeParsing = 0x80018001,
			DesktopAbsoluteParsing = 0x80028000,
			ParentRelativeEditing = 0x80031001,
			DesktopAbsoluteEditing = 0x8004c000,
			FileSysPath = 0x80058000,
			URL = 0x80068000,
			ParentRelativeForAddressBar = 0x8007c001,
			ParentRelative = 0x80080001
		}

		[ComImport]
		[Guid ( "43826D1E-E718-42EE-BC55-A1E261C37BFE" )]
		[InterfaceType ( ComInterfaceType.InterfaceIsIUnknown )]
		public interface IShellItem
		{
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void BindToHandler ( [In, MarshalAs ( UnmanagedType.Interface )] IntPtr pbc, [In] ref Guid bhid, [In] ref Guid riid, out IntPtr ppv );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void GetParent ( [MarshalAs ( UnmanagedType.Interface )] out IShellItem ppsi );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void GetDisplayName ( [In] ShellItemGetDisplayName sigdnName, [MarshalAs ( UnmanagedType.LPWStr )] out string ppszName );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void GetAttributes ( [In] uint sfgaoMask, out uint psfgaoAttribs );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void Compare ( [In, MarshalAs ( UnmanagedType.Interface )] IShellItem psi, [In] uint hint, out int piOrder );
		}
		
		[StructLayout ( LayoutKind.Sequential, Pack = 4 )]
		public struct PropertyKey
		{
			public Guid FormatId;
			public uint PropertyId;
		}

		public enum ShellItemAttributeFlags
		{
			And = 0x00000001,
			Or = 0x00000002,
			AppCompat = 0x00000003,
		}

		[ComImport]
		[Guid ( "B63EA76D-1F85-456F-A19C-48159EFA858B" )]
		[InterfaceType ( ComInterfaceType.InterfaceIsIUnknown )]
		public interface IShellItemArray
		{
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void BindToHandler ( [In, MarshalAs ( UnmanagedType.Interface )] IntPtr pbc, [In] ref Guid rbhid, [In] ref Guid riid, out IntPtr ppvOut );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void GetPropertyStore ( [In] int Flags, [In] ref Guid riid, out IntPtr ppv );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void GetPropertyDescriptionList ( [In] ref PropertyKey keyType, [In] ref Guid riid, out IntPtr ppv );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void GetAttributes ( [In] ShellItemAttributeFlags dwAttribFlags, [In] uint sfgaoMask, out uint psfgaoAttribs );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void GetCount ( out uint pdwNumItems );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void GetItemAt ( [In] uint dwIndex, [MarshalAs ( UnmanagedType.Interface )] out IShellItem ppsi );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void EnumItems ( [MarshalAs ( UnmanagedType.Interface )] out IntPtr ppenumShellItems );
		}

		[DllImport ( "shell32.dll", CharSet = CharSet.Unicode, PreserveSig = false )]
		public static extern void SHCreateItemFromParsingName (
		[In, MarshalAs ( UnmanagedType.LPWStr )] string pszPath,
		[In] IntPtr pbc,
		[In, MarshalAs ( UnmanagedType.LPStruct )] Guid iIdIShellItem,
		[Out, MarshalAs ( UnmanagedType.Interface, IidParameterIndex = 2 )] out IShellItem iShellItem );

		[ComImport,
			Guid ( "947aab5f-0a5c-4c13-b4d6-4bf7836fc9f8" ),
			InterfaceType ( ComInterfaceType.InterfaceIsIUnknown )]
		public interface IFileOperation
		{
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void Advise ( IntPtr pfops, IntPtr pdwCookie );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void Unadvise ( uint dwCookie );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void SetOperationFlags ( OperationFlag dwOperationFlags );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void SetProgressMessage ( [MarshalAs ( UnmanagedType.LPWStr )]string pszMessage );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void SetProgressDialog ( IntPtr popd );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void SetProperties ( IntPtr pproparray );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void SetOwnerWindow ( IntPtr hwndOwner );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void ApplyPropertiesToItem ( IShellItem psiItem );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void ApplyPropertiesToItems ( IntPtr punkItems );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			long RenameItem ( IShellItem psiItem, [MarshalAs ( UnmanagedType.LPWStr )] string pszNewName, IntPtr pfopsItem );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			long RenameItems ( IntPtr pUnkItems, [MarshalAs ( UnmanagedType.LPWStr )] string pszNewName );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			long MoveItem ( IShellItem psiItem, IShellItem psiDestinationFolder,
				 [MarshalAs ( UnmanagedType.LPWStr )]string pszNewName, IntPtr pfopsItem );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			long MoveItems ( IntPtr punkItems, IShellItem psiDestinationFolder );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			long CopyItem ( IShellItem psiItem, IShellItem psiDestinationFolder,
				 [MarshalAs ( UnmanagedType.LPWStr )]string pszCopyName, IntPtr pfopsItem );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			long CopyItems ( IntPtr punkItems, IShellItem psiDestinationFolder );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			long DeleteItem ( IShellItem psiItem, IntPtr pfopsItem );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void DeleteItems ( IntPtr punkItems );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void NewItem ( IShellItem psiDestinationFolder, uint dwFileAttributes, [MarshalAs ( UnmanagedType.LPWStr )] string pszName,
				 [MarshalAs ( UnmanagedType.LPWStr )] string pszTemplateName, IntPtr pfopsItem );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void PerformOperations ();
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void GetAnyOperationsAborted ( [MarshalAs ( UnmanagedType.Bool )] ref bool pfAnyOperationsAborted );
		}

#if !NOTASKDIALOG
		public enum TaskDialogNotification : uint
		{
			Created = 0,
			Navigated = 1,
			ButtonClicked = 2,
			HyperlinkClicked = 3,
			Timer = 4,
			Destroyed = 5,
			RadioButtonClicked = 6,
			DialogConstructed = 7,
			VerificationClicked = 8,
			Help = 9,
			ExpendOButtonClicked = 10,
		}

		public delegate long PFTASKDIALOGCALLBACK ( IntPtr hwnd,
			[MarshalAs ( UnmanagedType.U4 )] TaskDialogNotification msg,
			IntPtr wParam, IntPtr lParam,
			IntPtr lpRefData );

		[Flags]
		public enum TaskDialogFlags
		{
			EnableHyperlinks = 0x0001,
			UseHICONMain = 0x0002,
			UseHICONFooter = 0x0004,
			AllowDialogCancellation = 0x0008,
			UseCommandLinks = 0x0010,
			UseCommandLinksNoIcon = 0x0020,
			ExpandFooterArea = 0x0040,
			ExpandedByDefault = 0x0080,
			VerificationFlagChecked = 0x0100,
			ShowProgressBar = 0x0200,
			ShowMarqueeProgressBar = 0x0400,
			CallbackTimer = 0x0800,
			PositionRelativeToWindow = 0x1000,
			RTLLayout = 0x2000,
			NoDefaultRadioButton = 0x4000,
			CanBeMinimized = 0x8000,
			NoSetForeground = 0x00010000,
			SizeToContent = 0x01000000,
		}

		[StructLayout ( LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1 )]
		public struct TaskDialogConfig
		{
			public uint StructSize;
			public IntPtr ParentWindowHandle;
			public IntPtr InstanceHandle;
			public TaskDialogFlags Flags;
			public Dialogs.TaskDialogCommonButtonFlags CommonButtons;
			public string WindowTitle;
			public IntPtr MainIconHandle;
			public string MainInstruction;
			public string Content;
			public uint ButtonsCount;
			public IntPtr ButtonsArray;
			public int DefaultButton;
			public uint RadioButtonsCount;
			public IntPtr RadioButtonsArray;
			public int DefaultRadioButton;
			public string VerificationText;
			public string ExpandedInformation;
			public string ExpandedControlText;
			public string CollapsedControlText;
			public IntPtr FooterIconHandle;
			public string Footer;
			[MarshalAs ( UnmanagedType.FunctionPtr )]
			public PFTASKDIALOGCALLBACK Callback;
			public IntPtr CallbackData;
			public uint Width;
		}

		internal const int WM_USER = 0x0400;
		internal enum TaskDialogMessages : uint
		{
			NavigatePage = WM_USER + 101,
			ClickButton = WM_USER + 102,
			SetMarqueeProgressBar = WM_USER + 103,
			SetProgressBarState = WM_USER + 104,
			SetProgressBarRange = WM_USER + 105,
			SetProgressBarPos = WM_USER + 106,
			SetProgressBarMarquee = WM_USER + 107,
			SetElementText = WM_USER + 108,
			ClickRadioButton = WM_USER + 110,
			EnableButton = WM_USER + 111,
			EnableRadioButton = WM_USER + 112,
			ClickVerification = WM_USER + 113,
			UpdateElementText = WM_USER + 114,
			SetButtonElevationRequiredState = WM_USER + 115,
			UpdateIcon = WM_USER + 116,
		}

		[DllImport ( "user32.dll", CharSet = CharSet.Unicode )]
		public static extern long SendMessage ( IntPtr hWnd, TaskDialogMessages Msg, IntPtr wParam, ref TaskDialogConfig lParam );
		[DllImport ( "user32.dll", CharSet = CharSet.Unicode )]
		public static extern long SendMessage ( IntPtr hWnd, TaskDialogMessages Msg, IntPtr wParam, int lParam );
		[DllImport ( "user32.dll", CharSet = CharSet.Unicode )]
		public static extern long SendMessage ( IntPtr hWnd, TaskDialogMessages Msg, IntPtr wParam, IntPtr lParam );
		[DllImport ( "user32.dll", CharSet = CharSet.Unicode )]
		public static extern long SendMessage ( IntPtr hWnd, TaskDialogMessages Msg, IntPtr wParam, string lParam );
		[DllImport ( "user32.dll", CharSet = CharSet.Unicode )]
		public static extern long SendMessage ( IntPtr hWnd, TaskDialogMessages Msg, IntPtr wParam, uint lParam );

		[DllImport ( "kernel32.dll", CharSet = CharSet.Unicode )]
		static extern IntPtr LoadLibraryEx ( string lpFileName, IntPtr hFile, uint dwFlags );
		[DllImport ( "kernel32.dll", CharSet = CharSet.Unicode )]
		static extern IntPtr GetProcAddress ( IntPtr hModule, [MarshalAs ( UnmanagedType.LPStr )] string lpProcName );
		[DllImport ( "kernel32.dll", CharSet = CharSet.Unicode )]
		static extern bool FreeLibrary ( IntPtr hModule );

		delegate long TaskDialogDelegate ( IntPtr hwndOwner, IntPtr hInstance, [MarshalAs ( UnmanagedType.LPWStr )] string pszWindowTitle, [MarshalAs ( UnmanagedType.LPWStr )] string pszMainInstruction, [MarshalAs ( UnmanagedType.LPWStr )] string pszContent,
			Dialogs.TaskDialogCommonButtonFlags dwCommonButtons, IntPtr pszIcon, out int pnButton );
		delegate long TaskDialogIndirectDelegate ( ref TaskDialogConfig pTaskConfig, out int pnButton, out int pnRadioButton,
			[MarshalAs ( UnmanagedType.Bool )] out bool pfVerificationFlagChecked );

		class HMODULE
		{
			IntPtr module;
			public HMODULE ( IntPtr m ) { module = m; }
			~HMODULE () { FreeLibrary ( module ); }
			public static implicit operator IntPtr ( HMODULE m ) { return m.module; }
			public static implicit operator HMODULE ( IntPtr p ) { return new HMODULE ( p ); }
		}
		static HMODULE hModule;
		static TaskDialogDelegate taskDialogAction;
		static TaskDialogIndirectDelegate taskDialogIndirectAction;

		static void LoadTaskDialog ()
		{
			if ( !Utility.IsAvailablePlatform () )
				throw new PlatformNotSupportedException ();

			string systemDirectory = Environment.GetEnvironmentVariable ( "windir" );
			foreach ( string dir in Directory.EnumerateDirectories ( Path.Combine ( systemDirectory, "WinSxS" ), $"{( IntPtr.Size == 4 ? "x86" : "amd64" )}_microsoft.windows.common-controls_6595b64144ccf1df_6.*" ) )
			{
				hModule = LoadLibraryEx ( Path.Combine ( dir, "comctl32.dll" ), IntPtr.Zero, 0 );
				IntPtr taskDialog = GetProcAddress ( hModule, "TaskDialog" );
				IntPtr taskDialogIndirect = GetProcAddress ( hModule, "TaskDialogIndirect" );

				if ( taskDialog == null || taskDialogIndirect == null )
					throw new PlatformNotSupportedException ();

#pragma warning disable 0618
				taskDialogAction = Marshal.GetDelegateForFunctionPointer ( taskDialog, typeof ( TaskDialogDelegate ) ) as TaskDialogDelegate;
				taskDialogIndirectAction = Marshal.GetDelegateForFunctionPointer ( taskDialogIndirect, typeof ( TaskDialogIndirectDelegate ) ) as TaskDialogIndirectDelegate;
#pragma warning restore 0618

				break;
			}
		}

		public static long TaskDialog ( IntPtr OwnerWindowHandle, string windowTitle, string mainInstruction, string content,
			Dialogs.TaskDialogCommonButtonFlags commonButtons, IntPtr iconHandle, out int button )
		{
			if ( taskDialogAction == null )
				LoadTaskDialog ();
			return taskDialogAction ( OwnerWindowHandle, IntPtr.Zero, windowTitle, mainInstruction, content, commonButtons, iconHandle, out button );
		}
		public static long TaskDialogIndirect ( ref TaskDialogConfig taskConfig, out int button, out int radioButton,
			[MarshalAs ( UnmanagedType.Bool )] out bool verificationFlagChecked )
		{
			if ( taskDialogAction == null )
				LoadTaskDialog ();
			return taskDialogIndirectAction ( ref taskConfig, out button, out radioButton, out verificationFlagChecked );
		}
#endif

		[ComImport]
		[Guid ( "b4db1657-70d7-485e-8e3e-6fcb5a5c1802" )]
		[InterfaceType ( ComInterfaceType.InterfaceIsIUnknown )]
		public interface IModalWindow
		{
			[PreserveSig]
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			int Show ( [In] IntPtr parent );
		}

		[StructLayout ( LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4 )]
		public struct CommonDialogFilterSpecification
		{
			[MarshalAs ( UnmanagedType.LPWStr )]
			public string Name;
			[MarshalAs ( UnmanagedType.LPWStr )]
			public string Specification;
		}

		[Flags]
		public enum FileOpenDialogOptions : uint
		{
			None = 0x00000000,
			OverwritePrompt = 0x00000002,
			StrictFileTypes = 0x00000004,
			NoChangeDir = 0x00000008,
			PickFolders = 0x00000020,
			ForceFileSystem = 0x00000040,
			AllNonStorageItems = 0x00000080,
			NoValidate = 0x00000100,
			AllowMultiselect = 0x00000200,
			PathMustExist = 0x00000800,
			FileMustExist = 0x00001000,
			CreatePrompt = 0x00002000,
			ShareAware = 0x00004000,
			NoReadOnlyReturn = 0x00008000,
			NoTestFileCreate = 0x00010000,
			HideMRUPlaces = 0x00020000,
			HidePinnedPlaces = 0x00040000,
			NoDeReferenceLinks = 0x00100000,
			DontAddToRecent = 0x02000000,
			ForceShowHidden = 0x10000000,
			DefaultNoMiniMode = 0x20000000
		}

		public enum FDAP
		{
			Bottom = 0x00000000,
			Top = 0x00000001,
		}

		[ComImport]
		[Guid ( "42f85136-db7e-439c-85f1-e4075d135fc8" )]
		[InterfaceType ( ComInterfaceType.InterfaceIsIUnknown )]
		public interface IFileDialog : IModalWindow
		{
			#region IModalWindow
			[PreserveSig]
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new int Show ( [In] IntPtr parent );
			#endregion
			#region IFileDialog
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void SetFileTypes ( uint cFileTypes, [In, MarshalAs ( UnmanagedType.LPArray, SizeParamIndex = 0 )] CommonDialogFilterSpecification [] rgFilterSpec );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void SetFileTypeIndex ( uint iFileType );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void GetFileTypeIndex ( out uint piFileType );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void Advise ( IntPtr pfde, out uint pdwCookie );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void Unadvise ( [In] uint dwCookie );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void SetOptions ( [In] FileOpenDialogOptions fos );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void GetOptions ( out FileOpenDialogOptions pfos );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void SetDefaultFolder ( [In, MarshalAs ( UnmanagedType.Interface )] IShellItem psi );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void SetFolder ( [In, MarshalAs ( UnmanagedType.Interface )] IShellItem psi );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void GetFolder ( [MarshalAs ( UnmanagedType.Interface )] out IShellItem ppsi );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void GetCurrentSelection ( [MarshalAs ( UnmanagedType.Interface )] out IShellItem ppsi );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void SetFileName ( [In, MarshalAs ( UnmanagedType.LPWStr )] string pszName );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void GetFileName ( [MarshalAs ( UnmanagedType.LPWStr )] out string pszName );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void SetTitle ( [In, MarshalAs ( UnmanagedType.LPWStr )] string pszTitle );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void SetOkButtonLabel ( [In, MarshalAs ( UnmanagedType.LPWStr )] string pszText );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void SetFileNameLabel ( [In, MarshalAs ( UnmanagedType.LPWStr )] string pszLabel );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void GetResult ( [MarshalAs ( UnmanagedType.Interface )] out IShellItem ppsi );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void AddPlace ( [In, MarshalAs ( UnmanagedType.Interface )] IShellItem psi, FDAP fdap );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void SetDefaultExtension ( [In, MarshalAs ( UnmanagedType.LPWStr )] string pszDefaultExtension );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void Close ( [MarshalAs ( UnmanagedType.Error )] int hr );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void SetClientGuid ( [In] ref Guid guid );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void ClearClientData ();
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void SetFilter ( [MarshalAs ( UnmanagedType.Interface )] IntPtr pFilter );
			#endregion
		}


		[ComImport (),
		Guid ( "d57c7288-d4ad-4768-be02-9d969532d960" ),
		InterfaceType ( ComInterfaceType.InterfaceIsIUnknown )]
		public interface IFileOpenDialog : IFileDialog
		{
			#region IModalWindow
			[PreserveSig]
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new int Show ( [In] IntPtr parent );
			#endregion
			#region IFileDialog
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void SetFileTypes ( uint cFileTypes, [In, MarshalAs ( UnmanagedType.LPArray, SizeParamIndex = 0 )] CommonDialogFilterSpecification [] rgFilterSpec );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void SetFileTypeIndex ( uint iFileType );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void GetFileTypeIndex ( out uint piFileType );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void Advise ( IntPtr pfde, out uint pdwCookie );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void Unadvise ( [In] uint dwCookie );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void SetOptions ( [In] FileOpenDialogOptions fos );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void GetOptions ( out FileOpenDialogOptions pfos );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void SetDefaultFolder ( [In, MarshalAs ( UnmanagedType.Interface )] IShellItem psi );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void SetFolder ( [In, MarshalAs ( UnmanagedType.Interface )] IShellItem psi );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void GetFolder ( [MarshalAs ( UnmanagedType.Interface )] out IShellItem ppsi );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void GetCurrentSelection ( [MarshalAs ( UnmanagedType.Interface )] out IShellItem ppsi );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void SetFileName ( [In, MarshalAs ( UnmanagedType.LPWStr )] string pszName );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void GetFileName ( [MarshalAs ( UnmanagedType.LPWStr )] out string pszName );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void SetTitle ( [In, MarshalAs ( UnmanagedType.LPWStr )] string pszTitle );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void SetOkButtonLabel ( [In, MarshalAs ( UnmanagedType.LPWStr )] string pszText );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void SetFileNameLabel ( [In, MarshalAs ( UnmanagedType.LPWStr )] string pszLabel );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void GetResult ( [MarshalAs ( UnmanagedType.Interface )] out IShellItem ppsi );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void AddPlace ( [In, MarshalAs ( UnmanagedType.Interface )] IShellItem psi, FDAP fdap );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void SetDefaultExtension ( [In, MarshalAs ( UnmanagedType.LPWStr )] string pszDefaultExtension );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void Close ( [MarshalAs ( UnmanagedType.Error )] int hr );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void SetClientGuid ( [In] ref Guid guid );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void ClearClientData ();
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void SetFilter ( [MarshalAs ( UnmanagedType.Interface )] IntPtr pFilter );
			#endregion
			#region IFileOpenDialog
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void GetResults ( [MarshalAs ( UnmanagedType.Interface )] out IShellItemArray ppenum );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void GetSelectedItems ( [MarshalAs ( UnmanagedType.Interface )] out IShellItemArray ppsai );
			#endregion
		}

		[ComImport]
		[Guid ( "84bccd23-5fde-4cdb-aea4-af64b83d78ab" )]
		[InterfaceType ( ComInterfaceType.InterfaceIsIUnknown )]
		public interface IFileSaveDialog : IFileDialog
		{
			#region IModalWindow
			[PreserveSig]
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new int Show ( [In] IntPtr parent );
			#endregion
			#region IFileDialog
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void SetFileTypes ( uint cFileTypes, [In, MarshalAs ( UnmanagedType.LPArray, SizeParamIndex = 0 )] CommonDialogFilterSpecification [] rgFilterSpec );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void SetFileTypeIndex ( uint iFileType );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void GetFileTypeIndex ( out uint piFileType );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void Advise ( IntPtr pfde, out uint pdwCookie );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void Unadvise ( [In] uint dwCookie );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void SetOptions ( [In] FileOpenDialogOptions fos );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void GetOptions ( out FileOpenDialogOptions pfos );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void SetDefaultFolder ( [In, MarshalAs ( UnmanagedType.Interface )] IShellItem psi );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void SetFolder ( [In, MarshalAs ( UnmanagedType.Interface )] IShellItem psi );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void GetFolder ( [MarshalAs ( UnmanagedType.Interface )] out IShellItem ppsi );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void GetCurrentSelection ( [MarshalAs ( UnmanagedType.Interface )] out IShellItem ppsi );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void SetFileName ( [In, MarshalAs ( UnmanagedType.LPWStr )] string pszName );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void GetFileName ( [MarshalAs ( UnmanagedType.LPWStr )] out string pszName );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void SetTitle ( [In, MarshalAs ( UnmanagedType.LPWStr )] string pszTitle );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void SetOkButtonLabel ( [In, MarshalAs ( UnmanagedType.LPWStr )] string pszText );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void SetFileNameLabel ( [In, MarshalAs ( UnmanagedType.LPWStr )] string pszLabel );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void GetResult ( [MarshalAs ( UnmanagedType.Interface )] out IShellItem ppsi );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void AddPlace ( [In, MarshalAs ( UnmanagedType.Interface )] IShellItem psi, FDAP fdap );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void SetDefaultExtension ( [In, MarshalAs ( UnmanagedType.LPWStr )] string pszDefaultExtension );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void Close ( [MarshalAs ( UnmanagedType.Error )] int hr );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void SetClientGuid ( [In] ref Guid guid );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void ClearClientData ();
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			new void SetFilter ( [MarshalAs ( UnmanagedType.Interface )] IntPtr pFilter );
			#endregion
			#region IFileSaveDialog
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void SetSaveAsItem ( [In, MarshalAs ( UnmanagedType.Interface )] IShellItem psi );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void SetProperties ( [In, MarshalAs ( UnmanagedType.Interface )] IntPtr pStore );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void SetCollectedProperties ( [In, MarshalAs ( UnmanagedType.Interface )] IntPtr pList, [In] int fAppendDefault );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void GetProperties ( [MarshalAs ( UnmanagedType.Interface )] out IntPtr ppStore );
			[MethodImpl ( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
			void ApplyProperties ( [In, MarshalAs ( UnmanagedType.Interface )] IShellItem psi, [In, MarshalAs ( UnmanagedType.Interface )] IntPtr pStore, [In] ref IntPtr hwnd, [In, MarshalAs ( UnmanagedType.Interface )] IntPtr pSink );
			#endregion
		}

		[StructLayout ( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
		public struct WIN32_FIND_DATA
		{
			public FileAttributes dwFileAttributes;
			public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
			public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
			public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
			public uint nFileSizeHigh;
			public uint nFileSizeLow;
			public uint dwReserved0;
			public uint dwReserved1;
			[MarshalAs ( UnmanagedType.ByValTStr, SizeConst = 260 )]
			public string cFileName;
			[MarshalAs ( UnmanagedType.ByValTStr, SizeConst = 14 )]
			public string cAlternateFileName;
		}


		[DllImport ( "Kernel32", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode )]
		public static extern IntPtr FindFirstFile ( string lpFileName, out WIN32_FIND_DATA lpFindFileData );
		[DllImport ( "Kernel32", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode )]
		public static extern bool FindNextFile ( IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData );
		[DllImport ( "Kernel32", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode )]
		public static extern bool FindClose ( IntPtr hFindFile );
		[DllImport ( "Shlwapi", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode )]
		public static extern bool PathMatchSpec ( string pszFile, string pszSpec );
	}
}
