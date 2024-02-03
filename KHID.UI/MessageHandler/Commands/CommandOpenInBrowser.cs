using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PhotinoNET;

namespace KHID.UI.MessageHandler.Commands;

/// <summary>
/// A command that opens an <c>url</c> in the OS default browser
/// </summary>
public class CommandOpenInBrowser(IServiceProvider serviceProvider) : ICommand
{
    private readonly ILogger<CommandOpenInBrowser> _logger = serviceProvider.GetRequiredService<ILogger<CommandOpenInBrowser>>();

    public async Task Execute(PhotinoWindow? sender, object? data)
    {
        if (data == null) return;
        var url = data.ToString();

        var openBrowserProcess = new Process();
        openBrowserProcess.StartInfo.UseShellExecute = true;
        openBrowserProcess.StartInfo.FileName = url;
        openBrowserProcess.Start();

        await Task.Yield();
    }
}