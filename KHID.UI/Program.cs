using System.Reflection;
using System.Text;
using KHID.UI.KHID.Shared;
using KHID.UI.MessageHandler;
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

        var messageHandler = new MessageHandler.MessageHandler();
        var settingsManager = SettingsManager.SettingsManager.GetInstance();
        var downloadQueue = new DownloadQueue();

        var window = new PhotinoWindow()
            .SetTitle("KHInsider Downloader")
            .SetMinSize(Convert.ToInt32(500 * ScreenScaleFactor.Get()), Convert.ToInt32(400 * ScreenScaleFactor.Get()))
            .SetSize(Convert.ToInt32(850 * ScreenScaleFactor.Get()), Convert.ToInt32(720 * ScreenScaleFactor.Get()))
            .SetUseOsDefaultSize(false)
            .Center()
            .SetLogVerbosity(0)
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
            Message response = new() {
                Command = "queue-updated-response",
                Data = list,
            };
            MessageHandler.MessageHandler.SendResponse(window, response);
        };
        downloadQueue.OnQueueItemProgressEventHandler += (sender, item) =>
        {
            Message response = new() {
                Command = "queue-item-progress-response",
                Data = item,
            };
            MessageHandler.MessageHandler.SendResponse(window, response);
        };

        window.WaitForClose();
    }
}
