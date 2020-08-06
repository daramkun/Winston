using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static Daramee.Winston.WinstonInterop;

namespace Daramee.Winston.Dialogs
{
#if !NOTASKDIALOG
	[Flags]
	public enum TaskDialogCommonButtonFlags
	{
		OK = 0x0001,
		Yes = 0x0002,
		No = 0x0004,
		Cancel = 0x0008,
		Retry = 0x0010,
		Close = 0x0020
	}

	[StructLayout ( LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1 )]
	public struct TaskDialogButton
	{
		public int ButtonID;
		public string ButtonText;

		public TaskDialogButton ( int buttonId, string buttonText )
		{
			ButtonID = buttonId;
			ButtonText = buttonText;
		}

		public override string ToString () => $"{{Id: {ButtonID}, Text: {ButtonText}}}";

		public static TaskDialogButton [] Cast ( string [] buttons )
		{
			List<TaskDialogButton> tdButtons = new List<TaskDialogButton> ();
			int id = 101;
			foreach ( var button in buttons )
			{
				TaskDialogButton b = new TaskDialogButton
				{
					ButtonID = id++,
					ButtonText = button
				};
				tdButtons.Add ( b );
			}
			return tdButtons.ToArray ();
		}
	}

	public class TaskDialogResult
	{
		public const int OK = 1;
		public const int Yes = 6;
		public const int No = 7;
		public const int Cancel = 2;
		public const int Retry = 4;

		public int Button { get; internal set; }
		public int RadioButton { get; internal set; }
		public bool Verification { get; internal set; }

		public override string ToString () => $"{{Button: {Button}, Radio Button: {RadioButton}, Verification: {Verification}}}";
	}

	public enum TaskDialogProgressBarState
	{
		Normal = 1,
		Error = 2,
		Pause = 3,
	}

	public enum TaskDialogElement
	{
		Content,
		ExpandedInformation,
		Footer,
		MainInstruction,
	}

	public enum TaskDialogIconType
	{
		Main,
		Footer
	}


	public class TaskDialogEventArgs : EventArgs
	{
		internal IntPtr TaskDialogHandle { get; set; }
		public int ReturnValue { get; set; } = 0;

		public void SetMarqueeProgressBar ( bool marquee )
		{
			SendMessage ( TaskDialogHandle, TaskDialogMessages.SetMarqueeProgressBar,
				new IntPtr ( marquee ? 1 : 0 ), 0 );
		}
		public void SetMarqueeProgressBar ( bool marquee, TimeSpan marqueeTime )
		{
			SendMessage ( TaskDialogHandle, TaskDialogMessages.SetProgressBarMarquee,
				new IntPtr ( marquee ? 1 : 0 ), ( uint ) marqueeTime.TotalMilliseconds );
		}
		public void SetProgressBarState ( TaskDialogProgressBarState state )
		{
			SendMessage ( TaskDialogHandle, TaskDialogMessages.SetProgressBarState,
				new IntPtr ( ( int ) state ), 0 );
		}
		public void SetProgressBarPosition ( int position )
		{
			SendMessage ( TaskDialogHandle, TaskDialogMessages.SetProgressBarPos,
				new IntPtr ( position ), 0 );
		}
		public void SetProgressBarRange ( ushort minimum, ushort maximum )
		{
			unchecked
			{
				uint lParam = ( ( ( uint ) maximum ) << 16 ) & ( uint ) ( 0xffff0000 ) + ( uint ) minimum;
				SendMessage ( TaskDialogHandle, TaskDialogMessages.SetProgressBarRange,
					IntPtr.Zero, lParam );
			}
		}
		public void SetButtonElevationRequiredState ( int buttonId, bool state )
		{
			SendMessage ( TaskDialogHandle, TaskDialogMessages.SetButtonElevationRequiredState,
				new IntPtr ( buttonId ), state ? 1 : 0 );
		}
		public void SetElementText ( TaskDialogElement element, string text )
		{
			SendMessage ( TaskDialogHandle, TaskDialogMessages.SetElementText,
				new IntPtr ( ( int ) element ), text );
		}
		public void UpdateElementText ( TaskDialogElement element, string text )
		{
			SendMessage ( TaskDialogHandle, TaskDialogMessages.UpdateElementText,
				new IntPtr ( ( int ) element ), text );
		}
		public void EnableButton ( int buttonId, bool enableState )
		{
			SendMessage ( TaskDialogHandle, TaskDialogMessages.EnableButton,
				new IntPtr ( buttonId ), enableState ? 1 : 0 );
		}
		public void EnableRadioButton ( int buttonId, bool enableState )
		{
			SendMessage ( TaskDialogHandle, TaskDialogMessages.EnableRadioButton,
				new IntPtr ( buttonId ), enableState ? 1 : 0 );
		}
		public void ClickButton ( int buttonId )
		{
			SendMessage ( TaskDialogHandle, TaskDialogMessages.ClickButton,
				new IntPtr ( buttonId ), 0 );
		}
		public void ClickRadioButton ( int buttonId )
		{
			SendMessage ( TaskDialogHandle, TaskDialogMessages.ClickRadioButton,
				new IntPtr ( buttonId ), 0 );
		}
		public void ClickVerification ( bool checkState, bool focus = true )
		{
			SendMessage ( TaskDialogHandle, TaskDialogMessages.ClickVerification,
				new IntPtr ( checkState ? 1 : 0 ), focus ? 1 : 0 );
		}
		public void NavigatePage ( TaskDialog taskDialog )
		{
			SendMessage ( TaskDialogHandle, TaskDialogMessages.NavigatePage,
				IntPtr.Zero, ref taskDialog.config );
		}
		public void UpdateIcon ( TaskDialogIconType iconType, TaskDialogIcon icon )
		{
			SendMessage ( TaskDialogHandle, TaskDialogMessages.UpdateIcon,
				new IntPtr ( ( int ) iconType ), icon.IconHandle );
		}
	}

	public class ButtonClickedEventArgs : TaskDialogEventArgs
	{
		public int ButtonId { get; private set; }
		public ButtonClickedEventArgs ( int buttonId ) { ButtonId = buttonId; }
	}

	public class ExpandButtonClickedEventArgs : TaskDialogEventArgs
	{
		public bool IsExpanded { get; private set; }
		public ExpandButtonClickedEventArgs ( bool isExpanded ) { IsExpanded = isExpanded; }
	}

	public class HyperLinkClickedEventArgs : TaskDialogEventArgs
	{
		public string HyperlinkUrl { get; private set; }
		public HyperLinkClickedEventArgs ( string hyperlinkUrl ) { HyperlinkUrl = hyperlinkUrl; }
	}

	public class TimerEventArgs : TaskDialogEventArgs
	{
		public TimeSpan ElapsedTime { get; private set; }
		public TimerEventArgs ( TimeSpan elapsedTime ) { ElapsedTime = elapsedTime; }
	}

	public class VerificationClickedEventArgs : TaskDialogEventArgs
	{
		public bool IsVerificationChecked { get; private set; }
		public VerificationClickedEventArgs ( bool isVerificationChecked ) { IsVerificationChecked = isVerificationChecked; }
	}

	public abstract class TaskDialogIcon
	{
		static IntPtr MAKEINTRESOURCE ( int resId ) { return new IntPtr ( ( int ) ( ulong ) ( ushort ) resId ); }

		public static readonly TaskDialogDefaultIcon Warning = new TaskDialogDefaultIcon ( MAKEINTRESOURCE ( -1 ) );
		public static readonly TaskDialogDefaultIcon Error = new TaskDialogDefaultIcon ( MAKEINTRESOURCE ( -2 ) );
		public static readonly TaskDialogDefaultIcon Information = new TaskDialogDefaultIcon ( MAKEINTRESOURCE ( -3 ) );
		public static readonly TaskDialogDefaultIcon Shield = new TaskDialogDefaultIcon ( MAKEINTRESOURCE ( -4 ) );

#if DRAWING
		public static TaskDialogNativeIcon FromIcon ( System.Drawing.Icon icon )
		{
			return new TaskDialogNativeIcon ( icon.Handle );
		}
		public static TaskDialogNativeIcon FromImage ( System.Drawing.Image image )
		{
			return new TaskDialogNativeIcon ( ( image as System.Drawing.Bitmap ).GetHicon () );
		}
#endif

		public abstract IntPtr IconHandle { get; }
		public bool IsNullIcon => IconHandle == IntPtr.Zero;
	}

	public class TaskDialogDefaultIcon : TaskDialogIcon
	{
		readonly IntPtr _icon;
		public override IntPtr IconHandle => _icon;

		internal TaskDialogDefaultIcon ( IntPtr icon )
		{
			_icon = icon;
		}
	}

	public class TaskDialogNativeIcon : TaskDialogIcon
	{
		readonly IntPtr _icon;
		public override IntPtr IconHandle => _icon;

		public TaskDialogNativeIcon ( IntPtr hIcon ) { _icon = hIcon; }
	}

	public class TaskDialog
	{
		internal TaskDialogConfig config = new TaskDialogConfig ();

		public TaskDialogCommonButtonFlags CommonButtons { get { return config.CommonButtons; } set { config.CommonButtons = value; } }
		public string Title { get { return config.WindowTitle; } set { config.WindowTitle = value; } }
		public TaskDialogIcon MainIcon
		{
			set
			{
				config.MainIconHandle = value != null ? value.IconHandle : IntPtr.Zero;
				UseHICONMain = value is TaskDialogNativeIcon;
			}
		}
		public string MainInstruction { get { return config.MainInstruction; } set { config.MainInstruction = value; } }
		public string Content { get { return config.Content; } set { config.Content = value; } }
		public TaskDialogButton [] Buttons { get; set; }
		public int DefaultButton { get { return config.DefaultButton; } set { config.DefaultButton = value; } }
		public TaskDialogButton [] RadioButtons { get; set; }
		public int DefaultRadioButton { get { return config.DefaultRadioButton; } set { config.DefaultRadioButton = value; } }
		public string VerificationText { get { return config.VerificationText; } set { config.VerificationText = value; } }
		public string ExpandedInformation { get { return config.ExpandedInformation; } set { config.ExpandedInformation = value; } }
		public string ExpandedControlText { get { return config.ExpandedControlText; } set { config.ExpandedControlText = value; } }
		public string CollapsedControlText { get { return config.CollapsedControlText; } set { config.CollapsedControlText = value; } }
		public TaskDialogIcon FooterIcon
		{
			set
			{
				config.FooterIconHandle = value != null ? value.IconHandle : IntPtr.Zero;
				UseHICONFooter = value is TaskDialogNativeIcon;
			}
		}
		public string Footer { get { return config.Footer; } set { config.Footer = value; } }
		public uint Width { get { return config.Width; } set { config.Width = value; } }

		private bool UseHICONMain
		{
			get { return config.Flags.HasFlag ( TaskDialogFlags.UseHICONMain ); }
			set { if ( value ) config.Flags |= TaskDialogFlags.UseHICONMain; else config.Flags &= ~TaskDialogFlags.UseHICONMain; }
		}
		private bool UseHICONFooter
		{
			get { return config.Flags.HasFlag ( TaskDialogFlags.UseHICONFooter ); }
			set { if ( value ) config.Flags |= TaskDialogFlags.UseHICONFooter; else config.Flags &= ~TaskDialogFlags.UseHICONFooter; }
		}
		public bool EnableHyperlinks
		{
			get { return config.Flags.HasFlag ( TaskDialogFlags.EnableHyperlinks ); }
			set { if ( value ) config.Flags |= TaskDialogFlags.EnableHyperlinks; else config.Flags &= ~TaskDialogFlags.EnableHyperlinks; }
		}
		public bool AllowDialogCancellation
		{
			get { return config.Flags.HasFlag ( TaskDialogFlags.AllowDialogCancellation ); }
			set { if ( value ) config.Flags |= TaskDialogFlags.AllowDialogCancellation; else config.Flags &= ~TaskDialogFlags.AllowDialogCancellation; }
		}
		public bool UseCommandLinks
		{
			get { return config.Flags.HasFlag ( TaskDialogFlags.UseCommandLinks ); }
			set { if ( value ) config.Flags |= TaskDialogFlags.UseCommandLinks; else config.Flags &= ~TaskDialogFlags.UseCommandLinks; }
		}
		public bool UseCommandLinksNoIcon
		{
			get { return config.Flags.HasFlag ( TaskDialogFlags.UseCommandLinksNoIcon ); }
			set { if ( value ) config.Flags |= TaskDialogFlags.UseCommandLinksNoIcon; else config.Flags &= ~TaskDialogFlags.UseCommandLinksNoIcon; }
		}
		public bool ExpandFooterArea
		{
			get { return config.Flags.HasFlag ( TaskDialogFlags.ExpandFooterArea ); }
			set { if ( value ) config.Flags |= TaskDialogFlags.ExpandFooterArea; else config.Flags &= ~TaskDialogFlags.ExpandFooterArea; }
		}
		public bool ExpandedByDefault
		{
			get { return config.Flags.HasFlag ( TaskDialogFlags.ExpandedByDefault ); }
			set { if ( value ) config.Flags |= TaskDialogFlags.ExpandedByDefault; else config.Flags &= ~TaskDialogFlags.ExpandedByDefault; }
		}
		public bool VerificationFlagChecked
		{
			get { return config.Flags.HasFlag ( TaskDialogFlags.VerificationFlagChecked ); }
			set { if ( value ) config.Flags |= TaskDialogFlags.VerificationFlagChecked; else config.Flags &= ~TaskDialogFlags.VerificationFlagChecked; }
		}
		public bool ShowProgressBar
		{
			get { return config.Flags.HasFlag ( TaskDialogFlags.ShowProgressBar ); }
			set { if ( value ) config.Flags |= TaskDialogFlags.ShowProgressBar; else config.Flags &= ~TaskDialogFlags.ShowProgressBar; }
		}
		public bool ShowMarqueeProgressBar
		{
			get { return config.Flags.HasFlag ( TaskDialogFlags.ShowMarqueeProgressBar ); }
			set { if ( value ) config.Flags |= TaskDialogFlags.ShowMarqueeProgressBar; else config.Flags &= ~TaskDialogFlags.ShowMarqueeProgressBar; }
		}
		public bool CallbackTimer
		{
			get { return config.Flags.HasFlag ( TaskDialogFlags.CallbackTimer ); }
			set { if ( value ) config.Flags |= TaskDialogFlags.CallbackTimer; else config.Flags &= ~TaskDialogFlags.CallbackTimer; }
		}
		public bool PositionRelativeToWindow
		{
			get { return config.Flags.HasFlag ( TaskDialogFlags.PositionRelativeToWindow ); }
			set { if ( value ) config.Flags |= TaskDialogFlags.PositionRelativeToWindow; else config.Flags &= ~TaskDialogFlags.PositionRelativeToWindow; }
		}
		public bool RTLLayout
		{
			get { return config.Flags.HasFlag ( TaskDialogFlags.RTLLayout ); }
			set { if ( value ) config.Flags |= TaskDialogFlags.RTLLayout; else config.Flags &= ~TaskDialogFlags.RTLLayout; }
		}
		public bool NoDefaultRadioButton
		{
			get { return config.Flags.HasFlag ( TaskDialogFlags.NoDefaultRadioButton ); }
			set { if ( value ) config.Flags |= TaskDialogFlags.NoDefaultRadioButton; else config.Flags &= ~TaskDialogFlags.NoDefaultRadioButton; }
		}
		public bool CanBeMinimized
		{
			get { return config.Flags.HasFlag ( TaskDialogFlags.CanBeMinimized ); }
			set { if ( value ) config.Flags |= TaskDialogFlags.CanBeMinimized; else config.Flags &= ~TaskDialogFlags.CanBeMinimized; }
		}
		public bool NoSetForeground
		{
			get { return config.Flags.HasFlag ( TaskDialogFlags.NoSetForeground ); }
			set { if ( value ) config.Flags |= TaskDialogFlags.NoSetForeground; else config.Flags &= ~TaskDialogFlags.NoSetForeground; }
		}
		public bool SizeToContent
		{
			get { return config.Flags.HasFlag ( TaskDialogFlags.SizeToContent ); }
			set { if ( value ) config.Flags |= TaskDialogFlags.SizeToContent; else config.Flags &= ~TaskDialogFlags.SizeToContent; }
		}

		public event EventHandler<TaskDialogEventArgs> Created;
		public event EventHandler<TaskDialogEventArgs> Destroyed;
		public event EventHandler<TaskDialogEventArgs> Constructed;
		public event EventHandler<TaskDialogEventArgs> Help;
		public event EventHandler<ButtonClickedEventArgs> ButtonClicked;
		public event EventHandler<ButtonClickedEventArgs> RadioButtonClicked;
		public event EventHandler<HyperLinkClickedEventArgs> HyperlinkClicked;
		public event EventHandler<ExpandButtonClickedEventArgs> ExpandButtonClicked;
		public event EventHandler<VerificationClickedEventArgs> VerificationClicked;
		public event EventHandler<TaskDialogEventArgs> Navigated;
		public event EventHandler<TimerEventArgs> Timer;

		public TaskDialog ()
		{
			config.StructSize = ( uint ) Marshal.SizeOf ( config );
			config.Callback = ( IntPtr hwnd, TaskDialogNotification msg, IntPtr wParam, IntPtr lParam, IntPtr lpRefData ) =>
			{
				TaskDialogEventArgs commonEventArgs;
				switch ( msg )
				{
					case TaskDialogNotification.Created:
						{
							var eventArgs = new TaskDialogEventArgs () { TaskDialogHandle = hwnd };
							Created?.Invoke ( this, eventArgs );
							commonEventArgs = eventArgs;
						}
						break;
					case TaskDialogNotification.Destroyed:
						{
							var eventArgs = new TaskDialogEventArgs () { TaskDialogHandle = hwnd };
							Destroyed?.Invoke ( this, eventArgs );
							commonEventArgs = eventArgs;
						}
						break;
					case TaskDialogNotification.DialogConstructed:
						{
							var eventArgs = new TaskDialogEventArgs () { TaskDialogHandle = hwnd };
							Constructed?.Invoke ( this, eventArgs );
							commonEventArgs = eventArgs;
						}
						break;
					case TaskDialogNotification.Help:
						{
							var eventArgs = new TaskDialogEventArgs () { TaskDialogHandle = hwnd };
							Help?.Invoke ( this, eventArgs );
							commonEventArgs = eventArgs;
						}
						break;
					case TaskDialogNotification.ButtonClicked:
						{
							var eventArgs = new ButtonClickedEventArgs ( ( int ) wParam ) { TaskDialogHandle = hwnd };
							ButtonClicked?.Invoke ( this, eventArgs );
							commonEventArgs = eventArgs;
						}
						break;
					case TaskDialogNotification.RadioButtonClicked:
						{
							var eventArgs = new ButtonClickedEventArgs ( ( int ) wParam ) { TaskDialogHandle = hwnd };
							RadioButtonClicked?.Invoke ( this, eventArgs );
							commonEventArgs = eventArgs;
						}
						break;
					case TaskDialogNotification.HyperlinkClicked:
						{
							string url = null;
							unsafe { url = new string ( ( char* ) lParam ); }
							var eventArgs = new HyperLinkClickedEventArgs ( url ) { TaskDialogHandle = hwnd };
							HyperlinkClicked?.Invoke ( this, eventArgs );
							commonEventArgs = eventArgs;
						}
						break;
					case TaskDialogNotification.ExpendOButtonClicked:
						{
							var eventArgs = new ExpandButtonClickedEventArgs ( wParam.ToInt32 () == 0 ? false : true ) { TaskDialogHandle = hwnd };
							ExpandButtonClicked?.Invoke ( this, eventArgs );
							commonEventArgs = eventArgs;
						}
						break;
					case TaskDialogNotification.VerificationClicked:
						{
							var eventArgs = new VerificationClickedEventArgs ( wParam.ToInt32 () == 0 ? false : true ) { TaskDialogHandle = hwnd };
							VerificationClicked?.Invoke ( this, eventArgs );
							commonEventArgs = eventArgs;
						}
						break;
					case TaskDialogNotification.Navigated:
						{
							var eventArgs = new TaskDialogEventArgs () { TaskDialogHandle = hwnd };
							Navigated?.Invoke ( this, eventArgs );
							commonEventArgs = eventArgs;
						}
						break;
					case TaskDialogNotification.Timer:
						{
							var eventArgs = new TimerEventArgs ( TimeSpan.FromMilliseconds ( ( uint ) wParam ) ) { TaskDialogHandle = hwnd };
							Timer?.Invoke ( this, eventArgs );
							commonEventArgs = eventArgs;
						}
						break;

					default:
						return 0;
				}
				return commonEventArgs.ReturnValue;
			};
		}

		public static int Show ( IntPtr parentHwnd, string windowTitle, string mainInstruction, string content, TaskDialogCommonButtonFlags commonButtons, TaskDialogIcon icon )
		{
			long hr = WinstonInterop.TaskDialog ( parentHwnd, windowTitle, mainInstruction, content, commonButtons, icon.IconHandle, out int button );
			if ( 0 != hr )
				throw new ArgumentException ();
			return button;
		}
#if WINFORMS
		public static int Show ( System.Windows.Forms.Form parent, string windowTitle, string mainInstruction, string content, TaskDialogCommonButtonFlags commonButtons, TaskDialogIcon icon )
		{
			return Show ( parent.Handle, windowTitle, mainInstruction, content, commonButtons, icon );
		}
#endif
#if WPF
		public static int Show ( System.Windows.Window parent, string windowTitle, string mainInstruction, string content, TaskDialogCommonButtonFlags commonButtons, TaskDialogIcon icon )
		{
			return Show ( new System.Windows.Interop.WindowInteropHelper ( parent ).Handle, windowTitle, mainInstruction, content, commonButtons, icon );
		}
#endif
		public static int Show ( string windowTitle, string mainInstruction, string content, TaskDialogCommonButtonFlags commonButtons, TaskDialogIcon icon )
		{
			return Show ( IntPtr.Zero, windowTitle, mainInstruction, content, commonButtons, icon );
		}

		public TaskDialogResult Show ( IntPtr parent )
		{
			if ( Buttons != null )
			{
				int buttonStructSize = sizeof ( int ) + IntPtr.Size;
				IntPtr buttonsPtr = Marshal.AllocHGlobal ( buttonStructSize * Buttons.Length );
				IntPtr offset = buttonsPtr;
				foreach ( TaskDialogButton b in Buttons )
				{
					Marshal.WriteInt32 ( offset, b.ButtonID );
					offset += 4;
					Marshal.WriteIntPtr ( offset, Marshal.StringToHGlobalUni ( b.ButtonText ) );
					offset += IntPtr.Size;
				}
				config.ButtonsArray = buttonsPtr;
				config.ButtonsCount = ( uint ) Buttons.Length;
			}

			if ( RadioButtons != null )
			{
				int buttonStructSize = sizeof ( int ) + IntPtr.Size;
				IntPtr buttonsPtr = Marshal.AllocHGlobal ( buttonStructSize * RadioButtons.Length );
				IntPtr offset = buttonsPtr;
				foreach ( TaskDialogButton b in RadioButtons )
				{
					Marshal.WriteInt32 ( offset, b.ButtonID );
					offset += 4;
					Marshal.WriteIntPtr ( offset, Marshal.StringToHGlobalUni ( b.ButtonText ) );
					offset += IntPtr.Size;
				}
				config.RadioButtonsArray = buttonsPtr;
				config.RadioButtonsCount = ( uint ) RadioButtons.Length;
			}
			config.ParentWindowHandle = parent;

			long hr = TaskDialogIndirect ( ref config, out int button, out int radioButton, out bool verificationFlag );

			if ( config.RadioButtonsArray != IntPtr.Zero )
			{
				Marshal.FreeHGlobal ( config.RadioButtonsArray );
				config.RadioButtonsArray = IntPtr.Zero;
				config.RadioButtonsCount = 0;
			}
			if ( config.ButtonsArray != IntPtr.Zero )
			{
				Marshal.FreeHGlobal ( config.ButtonsArray );
				config.ButtonsArray = IntPtr.Zero;
				config.ButtonsCount = 0;
			}

			if ( 0 != hr )
				throw new ArgumentException ();

			return new TaskDialogResult () { Button = button, RadioButton = radioButton, Verification = verificationFlag };
		}
#if WINFORMS
		public TaskDialogResult Show ( System.Windows.Forms.Form parent )
		{
			return Show ( parent.Handle );
		}
#endif
#if WPF
		public TaskDialogResult Show ( System.Windows.Window parent )
		{
			return Show ( new System.Windows.Interop.WindowInteropHelper ( parent ).Handle );
		}
#endif
		public TaskDialogResult Show ()
		{
			return Show ( IntPtr.Zero );
		}
	}
#endif
}
