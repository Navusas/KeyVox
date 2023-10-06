using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace KeyVox.HotKeyListener;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IHotKeyService _hotKeyService;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            _hotKeyService = new WindowsHotKeyService();
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker started running at: {time}", DateTimeOffset.Now);
            if (_hotKeyService != null)
            {
                _hotKeyService.TryRegisterHotKey();
                _hotKeyService.HotKeyPressed += (sender, args) => 
                {
                    // Launch your Tauri application or any other task.
                    System.Diagnostics.Process.Start("/path/to/your/tauri/app");
                };

                // Keep the service running.
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }

                _hotKeyService.UnregisterHotKey();
            }

            _logger.LogInformation("Worker stopped running at: {time}", DateTimeOffset.Now);
        }
    }
}
