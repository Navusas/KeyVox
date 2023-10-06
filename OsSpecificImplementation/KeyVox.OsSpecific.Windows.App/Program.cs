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

                HotkeyManager.Current.AddOrReplace("ShiftZHotkey", Keys.Shift | Keys.Z, OnHotkeyPressed);

                Application.Run();
            }
        }

        private static void OnHotkeyPressed(object sender, HotkeyEventArgs e)
        {
            if (!_isOpen)
            {
                _isOpen = true;

                // Handle the hotkey press here, e.g., show a Tauri window or another form.
                MessageBox.Show("SHIFT + Z pressed!");

                _isOpen = false;

            }

            e.Handled = true;
        }
    }
}