using ElectronNET.API;

public static class ElectronBoostrap
{
    private static async void Bootstrap()
    {
        var browserWindow = await Electron.WindowManager.CreateWindowAsync();
        browserWindow.OnReadyToShow += () => browserWindow.Show();
        browserWindow.SetTitle("Electron.NET + Svelte");
    }
}