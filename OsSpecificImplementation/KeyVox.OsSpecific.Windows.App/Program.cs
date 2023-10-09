using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
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
            var iconStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("KeyVox.Startups.Windows.assets.logo.ico");
            if (iconStream != null)
            {
                var appIcon = new Icon(iconStream);
                icon.Icon = appIcon;
            }

            icon.Visible = true;
            icon.ContextMenuStrip = new ContextMenuStrip();
            icon.ContextMenuStrip.Items.Add("Exit", null, (s, e) => Application.Exit());

            var keyboard = Capture.Global.Keyboard();
            var hotkeyListener =
                new KeyChordEventSource(keyboard, new ChordClick(KeyCode.LControl, KeyCode.LShift, KeyCode.A));
            hotkeyListener.Triggered += async (x, y) => { await Listener_Triggered(keyboard, x, y); };
            hotkeyListener.Reset_On_Parent_EnabledChanged = false;
            hotkeyListener.Enabled = true;

            Application.Run();


            Application.ApplicationExit += (sender, args) =>
            {
                // clean-up
                keyboard.Dispose();
            };
        }

        private static async Task Listener_Triggered(IKeyboardEventSource keyboard, object? sender, KeyChordEventArgs e)
        {
            // using (keyboard.Suspend())
            // {
            await Simulate.Events()
                .Release(KeyCode.A)
                .Wait(1)
                .Invoke();

            // await Task.Delay(100);

            await Simulate.Events()
                .Release(KeyCode.LControlKey, KeyCode.LShiftKey)
                .Invoke();
            // }

            var captureSelection = await CaptureCurrentSelection(keyboard);
            var tempFile = StoreSelectedContextInTempFile(captureSelection);
            // MessageBox.Show(tempFile);
            await StartKeyVoxApp(tempFile);
        }

        private static async Task<string> CaptureCurrentSelection(IKeyboardEventSource keyboard)
        {
            IDataObject? clipboardBackup = null;
            string selectedText;
            try
            {
                clipboardBackup = Clipboard.GetDataObject(); // Backup current clipboard

                using (keyboard.Suspend())
                {
                    await Simulate.Events()
                        .ClickChord(KeyCode.LControl, KeyCode.C)
                        .Invoke();

                    await Task.Delay(200);
                }

                selectedText = GetClipboardTextWithRetry();
            }
            finally
            {
                if (clipboardBackup != null)
                {
                    Clipboard.SetDataObject(clipboardBackup, true, 20, 100);
                }
            }

            return selectedText;
        }

        private static string GetClipboardTextWithRetry(int retryCount = 20)
        {
            while (retryCount-- > 0)
            {
                try
                {
                    if (Clipboard.ContainsText())
                    {
                        return Clipboard.GetText();
                    }
                }
                catch (ExternalException)
                {
                    // Wait and retry
                    Thread.Sleep(100);
                }
            }

            return string.Empty; // let's assume that no context has been selected
        }

        private static string StoreSelectedContextInTempFile(string userQuery)
        {
            var tempFilePath = Path.GetTempFileName();
            File.WriteAllText(tempFilePath, userQuery);
            return tempFilePath;
        }

        private static async Task StartKeyVoxApp(string userQueryFilePath)
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

            var arguments = $"/k \"{cliExePath} --contextFile {userQueryFilePath}\"";

            // Start the KeyVox.Engine.Cli app
            var processStartInfo = new ProcessStartInfo("cmd.exe", arguments)
            {
                UseShellExecute = true
            };

            using var process = new Process();
            process.StartInfo = processStartInfo;

            process.Start();

            // Wait for the cmd process (and thus KeyVox.Engine.Cli app) to finish
            await process.WaitForExitAsync();
        }
    }
}