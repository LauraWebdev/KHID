using System.Diagnostics;
using PhotinoNET;

namespace KHID.UI.MessageHandler.Commands;

/// <summary>
/// A command that opens a <c>path</c> in the OS explorer/finder
/// </summary>
/// <remarks>
/// Does nothing if the <c>path</c> does not exist.
/// </remarks>
public class CommandOpenInExplorer : ICommand
{
    public async Task Execute(PhotinoWindow? sender, object? data)
    {
        if (data == null) return;
        var path = data.ToString();
        
        if (!Directory.Exists(path)) return;

        string cmd = Environment.OSVersion.Platform switch
        {
            PlatformID.Unix => "xdg-open",
            PlatformID.Win32NT => "explorer.exe",
            PlatformID.MacOSX => "open",
            _ => throw new Exception("Unknown platform")
        };

        var openExplorerProcess = new Process();
        openExplorerProcess.StartInfo.FileName = cmd;
        openExplorerProcess.StartInfo.Arguments = $@"""{path}""";
        openExplorerProcess.StartInfo.UseShellExecute = false;
        openExplorerProcess.StartInfo.CreateNoWindow = true;
        openExplorerProcess.Start();
        
        await Task.Yield();
    }
}