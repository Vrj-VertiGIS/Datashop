using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GEOCOM.GNSDatashop.Export.DXF.Common
{
    public class TaskDialog
    {
        public static DialogResult Show(IWin32Window parent, string message, Buttons buttons = Buttons.OK, Icon icon = Icon.Information)
            => Show(parent, message, Product.Name, string.Empty, string.Empty, string.Empty, string.Empty, buttons, icon);
        public static DialogResult Show(string message, Buttons buttons = Buttons.OK, Icon icon = Icon.Information)
            => Show((IWin32Window)null, message, buttons, icon);

        public static DialogResult Show(IWin32Window parent, string message, string caption, Buttons buttons = Buttons.OK, Icon icon = Icon.Information)
            => Show(parent, message, caption, string.Empty, string.Empty, string.Empty, string.Empty, buttons, icon);
        public static DialogResult Show(string message, string caption, Buttons buttons = Buttons.OK, Icon icon = Icon.Information)
            => Show((IWin32Window)null, message, caption, buttons, icon);

        public static DialogResult Show(IWin32Window parent, string message, string caption, string content, Buttons buttons = Buttons.OK, Icon icon = Icon.Information)
            => Show(parent, message, caption, content, string.Empty, string.Empty, string.Empty, buttons, icon);
        public static DialogResult Show(string message, string caption, string content, Buttons buttons = Buttons.OK, Icon icon = Icon.Information)
            => Show((IWin32Window)null, message, caption, content, buttons, icon);

        public static DialogResult Show(IWin32Window owner, string message, string caption, string content, string additionalInfo, string expandForMore, string collapseForLess,
            Buttons buttons = Buttons.OK, Icon icon = Icon.Information)
        {
            var rscIcon = MakeIntRessource((int)icon);
            var config = new TASKDIALOGCONFIG
            {
                cbSize = (uint)Marshal.SizeOf(typeof(TASKDIALOGCONFIG)),
                hwndParent = OwnerHandle(owner),
                hInstance = IntPtr.Zero,
                pszWindowTitle = caption,
                pszMainInstruction = message,
                pszContent = content,
                pszExpandedControlText = !string.IsNullOrEmpty(additionalInfo) ? collapseForLess : string.Empty,
                pszCollapsedControlText = !string.IsNullOrEmpty(additionalInfo) ? expandForMore : string.Empty,
                hFooterIcon = !string.IsNullOrEmpty(additionalInfo) ? MakeIntRessource((int)Icon.Information) : IntPtr.Zero,
                pszExpandedInformation = additionalInfo,
                dwCommonButtons = buttons,
                hMainIcon = rscIcon,
                pfCallback = TaskDialogCallback,
                dwFlags = Flags.Hyperlinks | Flags.AllowCancellation // | TaskDialogFlags.UseMainIcon  
            };

            return Show(config);
        }
        public static DialogResult Show(string message, string caption, string content, string additionalInfo, Buttons buttons = Buttons.OK, Icon icon = Icon.Information)
            => Show((IWin32Window)null, message, caption, content, additionalInfo, string.Empty, string.Empty, buttons, icon);

        #region advanced use
        public static DialogResult Show(TASKDIALOGCONFIG config)
        {
            var hr = TaskDialogIndirect(ref config, out var pressedButton, out var selRadio, out var setVerification);
            if (IntPtr.Zero != hr)
                throw new COMException("TaskDialog Error", (int)hr);

            return (DialogResult)pressedButton;
        }
        #endregion

        #region private helpers
        private static IntPtr MakeIntRessource(int rid)
        {
            var i16 = (Int16)rid;
            var u16 = (UInt16)i16;
            UInt32 u32 = u16;
            return new IntPtr(u32);
        }

        private static IntPtr OwnerHandle(IWin32Window owner)
            => owner?.Handle ?? IntPtr.Zero;

        #endregion

        #region Task dialog callback

        private static IntPtr TaskDialogCallback([In] IntPtr hwnd, [In] Notification msg, [In] IntPtr wParam, [In] IntPtr lParam, [In] IntPtr refData)
        {
            switch (msg)
            {
                case Notification.TDN_HYPERLINK_CLICKED:
                    try
                    {
                        var cmdLine = Marshal.PtrToStringUni(lParam);
                        Process.Start(cmdLine, Product.Name);
                    }
                    catch (Exception exe)
                    {
                        Show(exe.Message, Buttons.OK, Icon.Error);
                    }
                    break;
            }
            return IntPtr.Zero;
        }
        #endregion

        #region TaskDialog API

        [DllImport("comctl32.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr TaskDialogIndirect
        (
            [In] ref TASKDIALOGCONFIG pTaskConfig,
            out int pnButton,
            out int pnRadioButton,
            [MarshalAs(UnmanagedType.Bool)] out bool pfverificationFlagChecked
        );

        public enum Icon
        {
            Warning = -1,
            Error = -2,
            Information = -3,
            Shield = -4
        }

        public enum DialogResult
        {
            Ok = 1,
            Cancel = 2,
            Abort = 3,
            Retry = 4,
            Ignore = 5,
            Yes = 6,
            No = 7,
            Close = 8,
            Help = 9
        }

        [Flags]
        public enum Buttons
        {
            OK = 0x0001, // selected control return value IDOK
            Yes = 0x0002, // selected control return value IDYES
            No = 0x0004, // selected control return value IDNO
            Cancel = 0x0008, // selected control return value IDCANCEL
            Retry = 0x0010, // selected control return value IDRETRY
            Close = 0x0020  // selected control return value IDCLOSE
        };

        [Flags]
        public enum Flags
        {
            /// <summary>Enables hyperlink processing for the strings specified in the pszContent, pszExpandedInformation and pszFooter members. When enabled, these members may point to strings that contain hyperlinks in the following form:
            /// <code><![CDATA[<A HREF = "executablestring" > Hyperlink Text</A>]]></code>
            /// <note type="warning">Enabling hyperlinks when using content from an unsafe source may cause security vulnerabilities.</note>
            /// <note>Task Dialogs will not actually execute any hyperlinks.Hyperlink execution must be handled in the callback function specified by pfCallback.For more details, see TaskDialogCallbackProc.</note></summary>
            Hyperlinks = 0x0001,

            /// <summary>Indicates that the dialog should use the icon referenced by the handle in the hMainIcon member as the primary icon in the task dialog. If this flag is specified, the pszMainIcon member is ignored.</summary>
            UseMainIcon = 0x0002,

            /// <summary>Indicates that the dialog should use the icon referenced by the handle in the hFooterIcon member as the footer icon in the task dialog. If this flag is specified, the pszFooterIcon member is ignored.</summary>
            UseFooterIcon = 0x0004,

            /// <summary>Indicates that the dialog should be able to be closed using Alt-F4, Escape, and the title bar's close button even if no cancel button is specified in either the dwCommonButtons or pButtons members.</summary>
            AllowCancellation = 0x0008,

            /// <summary>Indicates that the buttons specified in the pButtons member are to be displayed as command links (using a standard task dialog glyph) instead of push buttons. When using command links, all characters up to the first new line character in the pszButtonText member will be treated as the command link's main text, and the remainder will be treated as the command link's note. This flag is ignored if the cButtons member is zero.</summary>
            ButtonsAsCommandLinks = 0x0010,

            /// <summary>Indicates that the buttons specified in the pButtons member are to be displayed as command links (without a glyph) instead of push buttons. When using command links, all characters up to the first new line character in the pszButtonText member will be treated as the command link's main text, and the remainder will be treated as the command link's note. This flag is ignored if the cButtons member is zero.</summary>
            ButtonsAsCommandLinksNoIcons = 0x0020,

            /// <summary>Indicates that the string specified by the pszExpandedInformation member is displayed at the bottom of the dialog's footer area instead of immediately after the dialog's content. This flag is ignored if the pszExpandedInformation member is NULL.</summary>
            ExpandFooterArea = 0x0040,

            /// <summary>Indicates that the string specified by the pszExpandedInformation member is displayed when the dialog is initially displayed. This flag is ignored if the pszExpandedInformation member is NULL.</summary>
            ExpandedByDefault = 0x0080,

            /// <summary>Indicates that the verification checkbox in the dialog is checked when the dialog is initially displayed. This flag is ignored if the pszVerificationText parameter is NULL.</summary>
            VerificationFlagChecked = 0x0100,

            /// <summary>Indicates that a Progress Bar is to be displayed.</summary>
            ShowProgressbar = 0x0200,

            /// <summary>Indicates that an Marquee Progress Bar is to be displayed.</summary>
            ShowMarqueeProgressbar = 0x0400,

            /// <summary>Indicates that the task dialog's callback is to be called approximately every 200 milliseconds.</summary>
            CallbackTimer = 0x0800,

            /// <summary>Indicates that the task dialog is positioned (centered) relative to the window specified by hwndParent. If the flag is not supplied (or no hwndParent member is specified), the task dialog is positioned (centered) relative to the monitor.</summary>
            PositionRelativeToWindow = 0x1000,

            /// <summary>Indicates that text is displayed reading right to left.</summary>
            RTLLayout = 0x2000,

            /// <summary>Indicates that no default item will be selected.</summary>
            NoDefaultRedioButton = 0x4000,

            /// <summary>Indicates that the task dialog can be minimized.</summary>
            CanBeMinimized = 0x8000,

            /// <summary>Don't call SetForegroundWindow() when activating the dialog.</summary>
            DontForceForeground = 0x00010000,

            /// <summary>
            /// Indicates that the width of the task dialog is determined by the width of its content area. This flag is ignored if cxWidth is not set to 0.
            /// </summary>
            SizeToContent = 0x01000000
        }

        public enum Notification
        {
            TDN_CREATED = 0,
            TDN_NAVIGATED = 1,
            TDN_BUTTON_CLICKED = 2,            // wParam = Button ID
            TDN_HYPERLINK_CLICKED = 3,            // lParam = (LPCWSTR)pszHREF
            TDN_TIMER = 4,            // wParam = Milliseconds since dialog created or timer reset
            TDN_DESTROYED = 5,
            TDN_RADIO_BUTTON_CLICKED = 6,            // wParam = Radio Button ID
            TDN_DIALOG_CONSTRUCTED = 7,
            TDN_VERIFICATION_CLICKED = 8,             // wParam = 1 if checkbox checked, 0 if not, lParam is unused and always 0
            TDN_HELP = 9,
            TDN_EXPANDO_BUTTON_CLICKED = 10            // wParam = 0 (dialog is now collapsed), wParam != 0 (dialog is now expanded)
        }

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate IntPtr TaskDialogCallbackProc([In] IntPtr hwnd, [In] Notification msg, [In] IntPtr wParam, [In] IntPtr lParam, [In] IntPtr refData);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct TASKDIALOGCONFIG
        {
            public uint cbSize;
            public IntPtr hwndParent;
            public IntPtr hInstance;
            public Flags dwFlags;
            public Buttons dwCommonButtons;
            public string pszWindowTitle;      // This can also be IntPtr if you need to pass in a Resource ID
            public IntPtr hMainIcon;
            public string pszMainInstruction;      // This can also be IntPtr if you need to pass in a Resource ID
            public string pszContent;          // This can also be IntPtr if you need to pass in a Resource ID
            public uint cButtons;
            public IntPtr pButtons;
            public int nDefaultButton;
            public uint cRadioButtons;
            public IntPtr pRadioButtons;
            public int nDefaultRadioButton;
            public string pszVerificationText;     // This can also be IntPtr if you need to pass in a Resource ID
            public string pszExpandedInformation;  // This can also be IntPtr if you need to pass in a Resource ID
            public string pszExpandedControlText;  // This can also be IntPtr if you need to pass in a Resource ID
            public string pszCollapsedControlText; // This can also be IntPtr if you need to pass in a Resource ID
            public IntPtr hFooterIcon;
            public string pszFooter;           // This can also be IntPtr if you need to pass in a Resource ID
            public TaskDialogCallbackProc pfCallback;
            public IntPtr lpCallbackData;
            public uint cxWidth;
        }

        #endregion
    }
}
