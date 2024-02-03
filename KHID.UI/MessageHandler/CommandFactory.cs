using System.Windows.Input;
using KHID.UI.MessageHandler.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KHID.UI.MessageHandler;


/// <summary>
/// The CommandFactory class is responsible for producing instances of ICommand, based on a provided string.
/// </summary>
public class CommandFactory
{
    /// <summary>
    /// Returns an instance of a class that implements <see cref="ICommand"/>, corresponding to the provided <c>command</c> string.
    /// </summary>
    /// <param name="command">Command as <see cref="String"/></param>
    /// <returns>An instance of a class implementing <see cref="ICommand"/> that corresponds to the <c>command</c> string.</returns>
    /// <exception cref="Exception">Thrown when an unknown command string is provided.</exception>
    public ICommand GetCommand(string command)
    {
        using var serviceProvider = new ServiceCollection()
            .AddLogging(configure => configure.AddConsole())
            .AddLogging(configure => configure.AddDebug())
            .BuildServiceProvider();

        return command switch
        {
            "open-in-browser" => new CommandOpenInBrowser(serviceProvider),
            "open-in-explorer" => new CommandOpenInExplorer(),
            "settings-open-in-explorer" => new CommandSettingsOpenInExplorer(),
            "settings-get" => new CommandSettingsGet(serviceProvider),
            "settings-get-full" => new CommandSettingsGetFull(serviceProvider),
            "settings-set" => new CommandSettingsSet(serviceProvider),
            "soundtrack-get" => new CommandSoundtrackGet(serviceProvider),
            _ => throw new Exception($"Unknown command: {command}")
        };
    }
}