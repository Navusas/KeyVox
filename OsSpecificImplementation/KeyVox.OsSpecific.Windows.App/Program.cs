using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace KeyVox.OsSpecific.Windows.App
{
    internal static class Program
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;

        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
        private static bool isSpacePressed = false;
        private static bool isShiftPressed = false;
        private static bool isShiftSpaceDetected = false;
        private static bool isArtificialCtrlC = false;

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (NotifyIcon icon = new NotifyIcon())
            {
                icon.Icon = SystemIcons.Exclamation;
                icon.Visible = true;
                icon.ContextMenuStrip = new ContextMenuStrip();
                icon.ContextMenuStrip.Items.Add("Exit", null, (s, e) => Application.Exit());

                _hookID = SetHook(_proc);

                ClipboardNotification.ClipboardUpdate += (sender, e) =>
                {
                    if (isShiftSpaceDetected)
                    {
                        string selectedText = Clipboard.GetText();
                        MessageBox.Show($"SHIFT + Space pressed. Selected Text: {selectedText}");
                        isShiftSpaceDetected = false;
                    }
                };

                Application.Run();

                UnhookWindowsHookEx(_hookID);
            }
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (isArtificialCtrlC)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if ((vkCode == (int)Keys.ControlKey) || (vkCode == (int)Keys.C))
                {
                    return CallNextHookEx(_hookID, nCode, wParam, lParam);
                }
                else
                {
                    isArtificialCtrlC = false; // If any other key is detected, reset the flag.
                }
            }

            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if (vkCode == (int)Keys.LShiftKey) isShiftPressed = true;
                if (vkCode == (int)Keys.Space) isSpacePressed = true;

                if (isShiftPressed && isSpacePressed)
                {
                    isShiftSpaceDetected = true;
                    SimulateCtrlC();
                    return (IntPtr)1;  // Suppress the space key press from being processed further
                }
            }
            else if (nCode >= 0 && wParam == (IntPtr)0x0101) // WM_KEYUP
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if (vkCode == (int)Keys.LShiftKey) isShiftPressed = false;
                if (vkCode == (int)Keys.Space) isSpacePressed = false;
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private static void SimulateCtrlC()
        {
            isArtificialCtrlC = true; // Set the flag

            keybd_event((byte)Keys.ControlKey, 0, 0, 0);
            keybd_event((byte)Keys.C, 0, 0, 0);
            keybd_event((byte)Keys.C, 0, 0x02, 0);
            keybd_event((byte)Keys.ControlKey, 0, 0x02, 0);
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        public static class ClipboardNotification
        {
            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool AddClipboardFormatListener(IntPtr hwnd);

            public static event EventHandler ClipboardUpdate;

            private static NotificationForm _form = new NotificationForm();

            private class NotificationForm : Form
            {
                public NotificationForm()
                {
                    // Turn the form invisible
                    this.ShowInTaskbar = false;
                    this.FormBorderStyle = FormBorderStyle.None;
                    this.Load += (s, e) => this.Size = new System.Drawing.Size(0, 0);

                    AddClipboardFormatListener(this.Handle);
                }

                protected override void WndProc(ref Message m)
                {
                    if (m.Msg == 0x031D) // WM_CLIPBOARDUPDATE
                    {
                        ClipboardUpdate?.Invoke(null, EventArgs.Empty);
                    }
                    base.WndProc(ref m);
                }
            }
        }
    }
}
