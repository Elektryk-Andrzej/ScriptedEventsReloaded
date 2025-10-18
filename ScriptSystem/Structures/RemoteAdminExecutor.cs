using RemoteAdmin;

namespace SER.ScriptSystem.Structures;

public class RemoteAdminExecutor : ScriptExecutor
{
    public required PlayerCommandSender Sender { get; init; }

    public override void Reply(string content, Script scr)
    {
        Sender.Print(content);
    }

    public override void Warn(string content, Script scr)
    {
        Sender.Print(
            $"<color=yellow>[WARN]</color> " +
            $"[Script {scr.Name}] " +
            $"[{(scr.CurrentLine == 0 ? "Compile warning" : $"Line {scr.CurrentLine}")}] " +
            $"{content}");
    }

    public override void Error(string content, Script scr)
    {
        Sender.Print(
            $"<color=red>[ERROR]</color> " +
            $"[Script {scr.Name}] " +
            $"[{(scr.CurrentLine == 0 ? "Compile error" : $"Line {scr.CurrentLine}")}] " +
            $"{content}");
    }
}