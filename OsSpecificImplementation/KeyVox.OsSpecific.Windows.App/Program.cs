using NHotkey.WindowsForms;
using NHotkey;

namespace KeyVox.OsSpecific.Windows.App
{
    internal static class Program
    {
        private static bool _isOpen = false;

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (NotifyIcon icon = new NotifyIcon())
            {
                icon.Icon = SystemIcons.Exclamation; // Use your custom icon here.
                icon.Visible = true;
                icon.ContextMenuStrip = new ContextMenuStrip();
                icon.ContextMenuStrip.Items.Add("Exit", null, (s, e) => Application.Exit());

                HotkeyManager.Current.AddOrReplace("ShiftZHotkey", Keys.Alt | Keys.A, OnHotkeyPressed);

                Application.Run();
            }
        }

        private static void OnHotkeyPressed(object sender, HotkeyEventArgs e)
        {
            if (!_isOpen)
            {
                _isOpen = true;

                string selectedText = GetCurrentlySelectedText();

                // Handle the hotkey press here, e.g., show a Tauri window or another form.
                MessageBox.Show($"SHIFT + Z pressed!\nSelected Text: {selectedText}");

                _isOpen = false;
            }

            e.Handled = true;
        }

        private static string GetCurrentlySelectedText()
        {
            // Backup the current clipboard content
            IDataObject clipboardBackup = Clipboard.GetDataObject();
            string initialClipboardText = Clipboard.GetText();


            // Simulate Ctrl+C to copy selected text to clipboard
            SendKeys.SendWait("^c");


            //// Clipboard update takes time. We
            //// want to poll for clipboard update (up to a timeout)
            string selectedText = string.Empty;
            int attemptLimit = 200;
            int attempts = 200;
            for (int i = 0; i < attemptLimit; i++)
            {
                selectedText = Clipboard.GetText();
                if (selectedText != initialClipboardText)
                {
                    break;
                }
                Thread.Sleep(10);
                attempts--;
            }

            selectedText += "\n\n attempt " + attempts.ToString();


            // Restore clipboard from backup
            if (clipboardBackup != null)
                Clipboard.SetDataObject(clipboardBackup);

            return selectedText;
        }
    }
}
