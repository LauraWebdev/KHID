using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PhotinoNET;

namespace KHID.UI.MessageHandler.Commands;

/// <summary>
/// A command that returns the value of a setting given a <c>key</c>
/// </summary>
public class CommandSettingsGet(IServiceProvider serviceProvider) : ICommand
{
    private SettingsManager.SettingsManager? _settingsManager;
    
    private readonly ILogger<CommandSettingsGet> _logger = serviceProvider.GetRequiredService<ILogger<CommandSettingsGet>>();

    public async Task Execute(PhotinoWindow? sender, object? data)
    {
        if (data == null) return;
        _settingsManager = SettingsManager.SettingsManager.GetInstance();
        
        var key = data.ToString();
        if (key == null) return;
        
        var responseData = new Dictionary<string, object?> {
            ["key"] = key,
            ["data"] = _settingsManager.Get<object>(key)
        };

        Message response = new() {
            Command = "settings-get-response",
            Data = responseData
        };
        
        await Task.Yield();

        MessageHandler.SendResponse(sender, response);
    }
}