using System;
using System.Linq;
using CommandSystem;
using Query;
using RemoteAdmin;
using SER.Helpers.Exceptions;

namespace SER.ScriptSystem.Structures;

public abstract class ScriptExecutor
{
    public abstract void Reply(string content, Script scr);
    public abstract void Warn(string content, Script scr);
    public abstract void Error(string content, Script scr);

    public static ScriptExecutor Get() => ServerConsoleExecutor.Instance;

    public static ScriptExecutor Get(ICommandSender sender, ArraySegment<string> commandArgs)
    {
        if (sender is not PlayerCommandSender playerSender)
        {
            return ServerConsoleExecutor.Instance;
        }

        if (commandArgs.Array!.First().First() == '.')
        {
            return new PlayerConsoleExecutor()
            {
                Sender = playerSender.ReferenceHub
            };
        }

        return new RemoteAdminExecutor()
        {
            Sender = playerSender
        };
    }
}