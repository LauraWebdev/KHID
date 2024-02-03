using PhotinoNET;

namespace KHID.UI.MessageHandler;

/// <summary>
/// A command from the UI that executes code
/// </summary>
public interface ICommand
{
    Task Execute(PhotinoWindow? sender, object? data);
}