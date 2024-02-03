using PhotinoNET;

namespace KHID.UI.MessageHandler.Commands;

/// <summary>
/// A command that opens the settings <see cref="SettingsManager.GetAppFolder"/> in the OS explorer/finder
/// </summary>
public class CommandSettingsOpenInExplorer : ICommand
{
    public async Task Execute(PhotinoWindow? sender, object? data)
    {
        CommandOpenInExplorer command = new();
        await command.Execute(sender, SettingsManager.SettingsManager.GetAppFolder());
    }
}