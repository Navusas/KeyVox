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
            
            using var icon = new NotifyIcon();
            icon.Icon = SystemIcons.Hand;
            icon.Visible = true;
            icon.ContextMenuStrip = new ContextMenuStrip();
            icon.ContextMenuStrip.Items.Add("Exit", null, (s, e) => Application.Exit());

            using var keyboardAsync = Capture.Global.KeyboardAsync();
            var hotkeyListener = new KeyChordEventSource(keyboardAsync, new ChordClick(KeyCode.LControl, KeyCode.LShift, KeyCode.A));
            hotkeyListener.Triggered += (x, y) => { Listener_Triggered(keyboardAsync, x, y); };
            hotkeyListener.Reset_On_Parent_EnabledChanged = false;
            hotkeyListener.Enabled = true;

            Application.Run();
        }

        private static async void Listener_Triggered(IKeyboardEventSource keyboard, object sender, KeyChordEventArgs e)
        {
            await Simulate.Events()
                .Release(KeyCode.A, KeyCode.LControlKey, KeyCode.LShiftKey)
                .Invoke();

            var captureSelection = await CaptureCurrentSelection(keyboard);
            await RevertChangesIfMade(keyboard, captureSelection);
            MessageBox.Show(captureSelection);
        }

        private static async Task RevertChangesIfMade(IKeyboardEventSource keyboard, string previousSelection)
        {
            // wait very short amount of time. In case we replaced text,
            await Task.Delay(20);
            
            // capture again, and if we replaced the text, then do CTRL+Z (undo)
            var captureSelectionTwice = await CaptureCurrentSelection(keyboard);
            if (previousSelection != captureSelectionTwice)
            {
                await Simulate.Events()
                    .ClickChord(KeyCode.LControl, KeyCode.Z)
                    .Wait(10)
                    .Invoke();
            }
        }

        private static async Task<string> CaptureCurrentSelection(IKeyboardEventSource keyboard)
        {
            var clipboardBackup = Clipboard.GetDataObject(); // Backup current clipboard
            var initialClipboardText = Clipboard.GetText();

            using (keyboard.Suspend())
            {
                await Simulate.Events()
                    .ClickChord(KeyCode.LControl, KeyCode.C)
                    .Wait(10)
                    .Invoke();
            }

            string selectedText;
            var retries = 100;

            do
            {
                await Task.Delay(10);
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
