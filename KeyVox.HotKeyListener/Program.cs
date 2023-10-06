using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.WindowsServices;


IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(configHost =>
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            configHost.UseWindowsService();
        }
        // else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        // {
        //     configHost.UseSystemd();
        // }
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
