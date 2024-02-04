using KHID.UI.KHID.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PhotinoNET;

namespace KHID.UI.MessageHandler.Commands;

public class CommandSoundtrackGet(IServiceProvider serviceProvider) : ICommand
{
    private readonly ILogger<CommandSoundtrackGet> _logger = serviceProvider.GetRequiredService<ILogger<CommandSoundtrackGet>>();
    
    public async Task Execute(PhotinoWindow? sender, object? data)
    {
        if (data == null) return;
        var urlOrSlug = data.ToString();

        var soundtrack = await DownloadQueue.Instance.GetSoundtrackMeta(urlOrSlug);

        Message response = new() {
            Command = "soundtrack-get-response",
            Data = soundtrack
        };

        MessageHandler.SendResponse(sender, response);
    }
}