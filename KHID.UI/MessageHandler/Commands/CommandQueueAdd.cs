using KHID.UI.KHID.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PhotinoNET;

namespace KHID.UI.MessageHandler.Commands;

public class CommandQueueAdd(IServiceProvider serviceProvider) : ICommand
{
    private SettingsManager.SettingsManager? _settingsManager;
    
    private readonly ILogger<CommandQueueAdd> _logger = serviceProvider.GetRequiredService<ILogger<CommandQueueAdd>>();

    public async Task Execute(PhotinoWindow? sender, object? data)
    {
        if (data == null) return;
        _settingsManager = SettingsManager.SettingsManager.GetInstance();

        var dataArray = (JArray)data;
        foreach (var jToken in dataArray)
        {
            var dataItem = (JObject)jToken;

            if (!dataItem.TryGetValue("Title", out var titleToken) || titleToken is not JValue) continue;
            if (!dataItem.TryGetValue("Url", out var urlToken) || urlToken is not JValue) continue;
            if (!dataItem.TryGetValue("SoundtrackTitle", out var soundtrackTitleToken) || soundtrackTitleToken is not JValue) continue;
            if (!dataItem.TryGetValue("OutputPath", out var outputPathToken) || outputPathToken is not JValue) continue;
            if (!dataItem.TryGetValue("Format", out var formatToken) || formatToken is not JValue) continue;

            var queueItem = new QueueItem
            {
                Title = titleToken.ToObject<string>(),
                Url = new Uri(urlToken.ToObject<string>()),
                SoundtrackTitle = soundtrackTitleToken.ToObject<string>(),
                OutputPath = outputPathToken.ToObject<string>(),
                Format = formatToken.ToObject<string>()
            };

            DownloadQueue.Instance.AddToQueue(queueItem, true);
        }
        
        DownloadQueue.Instance.OnQueueUpdatedEventHandler?.Invoke(this, DownloadQueue.Instance.GetQueue());

        Message response = new() {
            Command = "queue-add-response",
        };
        
        await Task.Yield();

        MessageHandler.SendResponse(sender, response);
    }
}