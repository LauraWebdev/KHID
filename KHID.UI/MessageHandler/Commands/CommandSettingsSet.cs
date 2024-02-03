using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PhotinoNET;

namespace KHID.UI.MessageHandler.Commands;

/// <summary>
/// A command that sets a setting given a <c>key</c> and <c>value</c>
/// </summary>
public class CommandSettingsSet(IServiceProvider serviceProvider) : ICommand
{
    private SettingsManager.SettingsManager? _settingsManager;
    
    private readonly ILogger<CommandSettingsSet> _logger = serviceProvider.GetRequiredService<ILogger<CommandSettingsSet>>();

    public async Task Execute(PhotinoWindow? sender, object? data)
    {
        if (data == null) return;
        _settingsManager = SettingsManager.SettingsManager.GetInstance();

        var dataArray = (JsonArray)data;
        foreach (var jToken in dataArray)
        {
            if (jToken == null) continue;
            var dataItem = (JsonObject)jToken;
            
            var key = dataItem?["key"]?.GetValue<string>();
            var value = dataItem?["value"]?.GetValue<object>();

            if (key == null || value == null) continue;
            
            _logger.LogInformation("Writing Setting: {Key}", key);
            _settingsManager.Set(key, value);
        }

        Message response = new() {
            Command = "settings-set-response",
            Data = _settingsManager.GetFull()
        };
        
        await Task.Yield();

        MessageHandler.SendResponse(sender, response);
    }
}