using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PhotinoNET;

namespace KHID.UI.MessageHandler.Commands;

/// <summary>
/// A command that opens an OS folder picker to select the Spin Rhythm XD game location
/// </summary>
/// <remarks>
/// By default, the picker starts in the saved path or, if not available, in the default install location of Spin Rhythm XD
/// </remarks>
public class CommandOutputPathSelect(IServiceProvider serviceProvider) : ICommand
{
    private SettingsManager.SettingsManager? _settingsManager;
    
    private readonly ILogger<CommandOutputPathSelect> _logger = serviceProvider.GetRequiredService<ILogger<CommandOutputPathSelect>>();

    public async Task Execute(PhotinoWindow? sender, object? data)
    {
        _settingsManager = SettingsManager.SettingsManager.GetInstance();
        
        var defaultPath = _settingsManager.Get<string>("output.defaultPath") ?? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string[]? resultPath = sender?.ShowOpenFolder(
            "Output path",
            Directory.Exists(defaultPath) ? defaultPath : null, 
            false
        );
        
        await Task.Yield();

        if (resultPath?.Length == 1 && Directory.Exists(resultPath[0]))
        {
            Message response = new() {
                Command = "output-path-select-response",
                Data = resultPath[0]
            };
            
            MessageHandler.SendResponse(sender, response);
        }
    }
}