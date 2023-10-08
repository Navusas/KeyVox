using System;
using System.Diagnostics;
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

            using var keyboard = Capture.Global.Keyboard();
            var hotkeyListener = new KeyChordEventSource(keyboard, new ChordClick(KeyCode.LControl, KeyCode.LShift, KeyCode.A));
            hotkeyListener.Triggered += (x, y) => { Listener_Triggered(keyboard, x, y); };
            hotkeyListener.Reset_On_Parent_EnabledChanged = false;
            hotkeyListener.Enabled = true;

            Application.Run();
        }

        private static void Listener_Triggered(IKeyboardEventSource keyboard, object sender, KeyChordEventArgs e)
        {
            var releaseAResult = Simulate.Events()
                .Release(KeyCode.A)
                .Invoke()
                .ConfigureAwait(false).GetAwaiter().GetResult();

            var releaseCtrlResult = Simulate.Events()
                    .Release(KeyCode.LControlKey, KeyCode.LShiftKey)
                    .Invoke()
                    .ConfigureAwait(false).GetAwaiter().GetResult();

            var captureSelection = CaptureCurrentSelection(keyboard);
            MessageBox.Show(captureSelection);
            // await RevertChangesIfMade(keyboard, captureSelection);
            //
            StartKeyVoxApp(captureSelection);
        }

        private static void StartKeyVoxApp(string userSelection)
        {
            var path = "KeyVox.Engine.Cli.exe";
            #if DEVELOPMENT
                path = "../../../../../release/KeyVox.Engine.Cli.exe";
            #endif
            
            // Define the path to the KeyVox.Engine.Cli executable
            var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var cliExePath = Path.GetFullPath(Path.Combine(appDirectory, path));

            // Ensure the path is correct and the file exists
            if (!File.Exists(cliExePath))
            {
                throw new Exception($"Executable not found: {cliExePath}");
            }

            var sanitizedArg = EscapeDoubleQuotes(userSelection);
            var arguments = $"/k \"\"{cliExePath}\" --userQuery {sanitizedArg}\"";

            // Start the KeyVox.Engine.Cli app
            var processStartInfo = new ProcessStartInfo("cmd.exe", arguments)
            {
                UseShellExecute = true
            };

            using var process = new Process();
            process.StartInfo = processStartInfo;

            process.Start();
    
            // Wait for the cmd process (and thus KeyVox.Engine.Cli app) to finish
            process.WaitForExit();
        }


        private static void RevertChangesIfMade(IKeyboardEventSource keyboard, string previousSelection)
        {
            // wait very short amount of time. In case we replaced text,
            Task.Delay(20);
            
            // capture again, and if we replaced the text, then do CTRL+Z (undo)
            var captureSelectionTwice = CaptureCurrentSelection(keyboard);
            if (previousSelection != captureSelectionTwice)
            {
                Simulate.Events()
                    .ClickChord(KeyCode.LControl, KeyCode.Z)
                    .Wait(10)
                    .Invoke()
                    .ConfigureAwait(false).GetAwaiter().GetResult();            }
        }

        private static string CaptureCurrentSelection(IKeyboardEventSource keyboard)
        {
            var clipboardBackup = Clipboard.GetDataObject(); // Backup current clipboard
            var initialClipboardText = Clipboard.GetText();

            using (keyboard.Suspend())
            {
                Simulate.Events()
                    .ClickChord(KeyCode.LControl, KeyCode.C)
                    .Wait(10)
                    .Invoke()
                    .ConfigureAwait(false).GetAwaiter().GetResult();
            }

            string selectedText;
            var retries = 100;

            do
            {
                Task.Delay(10);
                selectedText = Clipboard.GetText();
            } while (selectedText == initialClipboardText && --retries > 0);

            // Restore original clipboard
            if (clipboardBackup != null)
            {
                Clipboard.SetDataObject(clipboardBackup, true);
            }

            return selectedText;
        }
        
        private static string EscapeDoubleQuotes(string arg)
        {
            // Check for already escaped quotes and avoid double-escaping them
            arg = arg.Replace("\\\"", "\"").Replace("\"", "\\\"");
            return "\"" + arg + "\"";
        }
    }
}
