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

        Message response;
        try
        {
            var soundtrack = await DownloadQueue.Instance.GetSoundtrackMeta(urlOrSlug);

            response = new Message
            {
                Command = "soundtrack-get-response",
                Data = soundtrack
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            
            response = new Message
            {
                Command = "soundtrack-get-response",
                Data = null
            };
        }

        MessageHandler.SendResponse(sender, response);
    }
}