public static class ElectronBoostrap {
    public static void Do() {
        private async void ElectronBootstrap()
{
            var browserWindow = await Electron.WindowManager.CreateWindowAsync();
            browserWindow.OnReadyToShow += () => browserWindow.Show();
            browserWindow.SetTitle("Electron.NET + Svelte");
        }
    }
}