using System;
using System.Collections.Generic;
using System.Linq;
using RemoteAdmin;
using SER.Helpers;
using SER.Helpers.Exceptions;
using SER.ScriptSystem.FlagSystem.Commands;

namespace SER.ScriptSystem.FlagSystem;

public partial class ScriptFlagHandler
{
    public static readonly List<CommandScriptBind> CommandBoundScripts = [];

    public record CommandScriptBind(CustomCommand Command, string ScriptName);
    
    private static void AssignCommandToScript(FlagInfo flagInfo)
    {
        if (flagInfo.InlineArguments.Length == 0)
        {
            Log.Warn(flagInfo.ScriptName, "Script is using a command flag, but no command name was provided.");
        }

        var command = new CustomCommand
        {
            Command = flagInfo.InlineArguments.First()
        };

        CommandBoundScripts.Add(new(command, flagInfo.ScriptName));
    }

    private static void AddCommandArguments(string[] args)
    {
        var bind = CommandBoundScripts.LastOrDefault();
        if (bind != null) bind.Command.Usage = args;
    }

    private static void AddCommandConsoleType(string[] args)
    {
        List<ConsoleType> types = [];

        foreach (var arg in args)
        {
            if (Enum.TryParse(arg, true, out ConsoleType consoleType))
            {
                types.Add(consoleType);
                continue;
            }
            
            Log.Warn(
                CommandBoundScripts.LastOrDefault()?.ScriptName ?? "unknown", 
                "Console type flag argument is invalid.");
        }
        
        var bind = CommandBoundScripts.LastOrDefault();
        if (bind != null) bind.Command.ConsoleTypes = types.ToArray();
    }

    private static void CommandClear()
    {
        foreach (var command in CommandBoundScripts.Select(x => x.Command))
        {
            foreach (var console in command.ConsoleTypes)
            {
                switch (console)
                {
                    case ConsoleType.Player:
                        QueryProcessor.DotCommandHandler.UnregisterCommand(command);
                        break;
                    case ConsoleType.Server:
                        GameCore.Console.singleton.ConsoleCommandHandler.UnregisterCommand(command);
                        break;
                    case ConsoleType.RemoteAdmin:
                        CommandProcessor.RemoteAdminCommandHandler.UnregisterCommand(command);
                        break;
                    default:
                        throw new DeveloperFuckupException();
                }
            }
        }
        
        CommandBoundScripts.Clear();
    }

    internal static void RegisterCommands()
    {
        foreach (var command in CommandBoundScripts.Select(x => x.Command))
        {
            foreach (var console in command.ConsoleTypes)
            {
                switch (console)
                {
                    case ConsoleType.Player:
                        QueryProcessor.DotCommandHandler.RegisterCommand(command);
                        break;
                    case ConsoleType.Server:
                        GameCore.Console.singleton.ConsoleCommandHandler.RegisterCommand(command);
                        break;
                    case ConsoleType.RemoteAdmin:
                        CommandProcessor.RemoteAdminCommandHandler.RegisterCommand(command);
                        break;
                    default:
                        throw new DeveloperFuckupException();
                }
            }
        }
    }
}