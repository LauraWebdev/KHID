using System.Reflection;
using System.Text;
using KHID.UI.KHID.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PhotinoNET;
using PhotinoNET.Server;

namespace KHID.UI;

class Program
{
    private static ILogger<Program>? _logger;
    
    [STAThread]
    static void Main(string[] args)
    {
        // Error Logging
        using var serviceProvider = new ServiceCollection()
            .AddLogging(configure => configure.AddConsole())
            .AddLogging(configure => configure.AddDebug())
            .BuildServiceProvider();
        _logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        
        // Setting working directory
        var entryAssemblyLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
        if (entryAssemblyLocation != null)
        {
            Directory.SetCurrentDirectory(entryAssemblyLocation);
        }
        
        PhotinoServer
            .CreateStaticFileServer(args, out var baseUrl)
            .RunAsync();

        MessageHandler.MessageHandler messageHandler = new MessageHandler.MessageHandler();
        SettingsManager.SettingsManager settingsManager = SettingsManager.SettingsManager.GetInstance();
        DownloadQueue downloadQueue = new DownloadQueue();

        var window = new PhotinoWindow()
            .SetTitle("KHInsider Downloader")
            .SetMinSize(Convert.ToInt32(500 * ScreenScaleFactor.Get()), Convert.ToInt32(400 * ScreenScaleFactor.Get()))
            .SetSize(Convert.ToInt32(850 * ScreenScaleFactor.Get()), Convert.ToInt32(720 * ScreenScaleFactor.Get()))
            .SetUseOsDefaultSize(false)
            .Center()
            .SetResizable(true)
            .RegisterCustomSchemeHandler("app", (object sender, string scheme, string url, out string contentType) =>
            {
                contentType = "text/javascript";
                return new MemoryStream(Encoding.UTF8.GetBytes(@"
                        (() =>{
                            window.setTimeout(() => {
                                alert(`🎉 Dynamically inserted JavaScript.`);
                            }, 1000);
                        })();
                    "));
            })
            .RegisterWebMessageReceivedHandler(messageHandler.RegisterWebMessageReceivedHandler);

#if DEBUG
        _logger.LogInformation("Debug Mode, starting dev site");
        
        window.SetDevToolsEnabled(true);
        window.Load(new Uri($"http://localhost:5173/"));
#else
        _logger.LogInformation("Production Mode, starting built site");
        window.Load($"{baseUrl}/index.html");
#endif

        downloadQueue.OnQueueUpdatedEventHandler += (sender, list) =>
        {
            _logger.LogInformation("OnQueueUpdated");
        };
        downloadQueue.OnQueueItemProgressEventHandler += (sender, item) =>
        {
            _logger.LogInformation("OnQueueItemProgress");
        };

        window.WaitForClose();
    }
}
