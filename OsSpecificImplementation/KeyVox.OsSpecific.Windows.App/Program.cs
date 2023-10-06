using WindowsInput;
using WindowsInput.Events.Sources;
using WindowsInput.Events;
using System.Runtime.InteropServices;

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
                icon.Icon = SystemIcons.Exclamation; // Use your custom icon here.
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
            IDataObject clipboardBackup = Clipboard.GetDataObject();
            string initialClipboardText = Clipboard.GetText();

            await Simulate.Events()
                .Release(KeyCode.LControl)
                .Release(KeyCode.LShift)
                .Invoke();

            await Simulate.Events()
                .ClickChord(KeyCode.LControl, KeyCode.C)
                .Invoke();

            string selectedText;
            int retries = 100;
            do
            {
                await Task.Delay(50);
                selectedText = Clipboard.GetText();
            } while (selectedText == initialClipboardText && --retries > 0);


            await Simulate.Events()
                .ClickChord(KeyCode.LControl, KeyCode.Z)
                .Invoke();
            MessageBox.Show($"SHIFT + Space pressed. It took {100 - retries} attemps\n\n\nSelected Text: {selectedText}");
        }


        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
    }
}
