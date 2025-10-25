using System;
using Log = SER.Helpers.Log;
using Logger = LabApi.Features.Console.Logger;

namespace SER.ScriptSystem.Structures;

public class ServerConsoleExecutor : ScriptExecutor
{
    private ServerConsoleExecutor()
    {
    }
    
    public static ServerConsoleExecutor Instance { get; } = new();

    public override void Reply(string content, Script scr)
    {
        Logger.Raw($"[Script '{scr.Name}'] {content}", ConsoleColor.Green);
    }

    public override void Warn(string content, Script scr)
    {
        Log.Warn(scr, $"[Script '{scr.Name}'] [{(scr.CurrentLine == 0 ? "Compile warning" : $"Line {scr.CurrentLine}")} {content}");
    }

    public override void Error(string content, Script scr)
    {
        Log.Error(scr.Name, $"[{(scr.CurrentLine == 0 ? "Compile error" : $"Line {scr.CurrentLine}")}] {content}");
    }
}