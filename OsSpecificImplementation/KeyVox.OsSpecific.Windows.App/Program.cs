using System;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Events.Sources;
using WindowsInput.Events;
using System.Threading.Tasks;

namespace KeyVox.OsSpecific.Windows.App
{
    internal static class Program
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (NotifyIcon icon = new NotifyIcon())
            {
                icon.Icon = SystemIcons.Shield; // Use your custom icon here.
                icon.Visible = true;
                icon.ContextMenuStrip = new ContextMenuStrip();
                icon.ContextMenuStrip.Items.Add("Exit", null, (s, e) => Application.Exit());

                using (var Keyboard = Capture.Global.KeyboardAsync())
                {
                    var Listener = new KeyChordEventSource(Keyboard, new ChordClick(KeyCode.LControl, KeyCode.LShift, KeyCode.A));
                    Listener.Triggered += (x, y) => Listener_Triggered(Keyboard, x, y);
                    Listener.Reset_On_Parent_EnabledChanged = false;
                    Listener.Enabled = true;

                    Application.Run();
                }
            }
        }

        private static async void Listener_Triggered(IKeyboardEventSource Keyboard, object sender, KeyChordEventArgs e)
        {
            using (Keyboard.Suspend())
            {
                await Simulate.Events()
                    .Release(KeyCode.A)
                    .Release(KeyCode.LControl)
                    .Release(KeyCode.LShift)
                    .Invoke();

                await Task.Delay(50);
            }

            var captureSelection = await CaptureCurrentSelection(Keyboard);

            MessageBox.Show(captureSelection);
        }

        private static async Task<string> CaptureCurrentSelection(IKeyboardEventSource Keyboard)
        {
            IDataObject clipboardBackup = Clipboard.GetDataObject(); // Backup current clipboard
            string initialClipboardText = Clipboard.GetText();

            using (Keyboard.Suspend())
            {
                await Simulate.Events()
                    .ClickChord(KeyCode.LControl, KeyCode.C)
                    .Invoke();
            }
            string selectedText;
            int retries = 40;

            do
            {
                await Task.Delay(50);
                selectedText = Clipboard.GetText();
            } while (selectedText == initialClipboardText && --retries > 0);

            // Restore original clipboard
            if (clipboardBackup != null)
            {
                Clipboard.SetDataObject(clipboardBackup, true);
            }

            return selectedText;
        }
    }
}
