using KHID.UI.KHID.Shared;

namespace KHID.UI.MessageHandler.Commands;

using PhotinoNET;

/// <summary>
/// A command that returns download queue
/// </summary>
public class CommandQueueGet(IServiceProvider serviceProvider) : ICommand
{
    public async Task Execute(PhotinoWindow? sender, object? data)
    {
        Message response = new() {
            Command = "queue-get-response",
            Data = DownloadQueue.Instance.GetQueue()
        };
        
        await Task.Yield();

        MessageHandler.SendResponse(sender, response);
    }
}