using System.Collections.Generic;
using CommandSystem;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Enums;
using RemoteAdmin;
using SER.Helpers.Exceptions;
using Logger = LabApi.Features.Console.Logger;

namespace SER.ScriptSystem.Structures;

public abstract class ScriptExecutor
{
    public abstract void Reply(string content, Script scr);
    public abstract void Warn(string content, Script scr);
    public abstract void Error(string content, Script scr);

    public static readonly Dictionary<ICommandSender, CommandType> UsedCommandTypes = [];
    
    public static ScriptExecutor Get() => ServerConsoleExecutor.Instance;

    public static ScriptExecutor Get(ICommandSender sender)
    {
        if (!UsedCommandTypes.TryGetValue(sender, out var type))
        {
            Logger.Warn("A command was sent, but cannot infer the command type used. Switching to server.");
            return ServerConsoleExecutor.Instance;
        }
        
        if (type == CommandType.Console) return ServerConsoleExecutor.Instance;

        if (sender is not PlayerCommandSender playerSender)
        {
            Logger.Warn("A presumed command was sent, but cannot infer the player who sent it. Switching to server.");
            return ServerConsoleExecutor.Instance;
        }

        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
        return type switch
        {
            CommandType.Client => new PlayerConsoleExecutor { Sender = playerSender.ReferenceHub },
            CommandType.RemoteAdmin => new RemoteAdminExecutor { Sender = playerSender },
            _ => throw new AndrzejFuckedUpException()
        };
    }

    public static void Initialize()
    {
        ServerEvents.CommandExecuting += OnCommandExecuting;
        UsedCommandTypes.Clear();
    }

    public static void Disable()
    {
        ServerEvents.CommandExecuting -= OnCommandExecuting;
        UsedCommandTypes.Clear();
    }

    public static void OnCommandExecuting(CommandExecutingEventArgs ev)
    {
        UsedCommandTypes[ev.Sender] = ev.CommandType;
    }
}