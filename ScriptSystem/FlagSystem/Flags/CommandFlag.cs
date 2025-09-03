using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using JetBrains.Annotations;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using RemoteAdmin;
using SER.Helpers.Exceptions;
using SER.Helpers.Extensions;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem.FlagSystem.Structures;
using SER.ScriptSystem.Structures;

namespace SER.ScriptSystem.FlagSystem.Flags;

[UsedImplicitly]
public class CommandFlag : Flag
{
    public override FlagType Type => FlagType.Command;

    public override FlagArgument? InlineArgument => FlagArgument.CommandName;

    public override Dictionary<FlagArgument, Func<string[], Result>> Arguments => new()
    {
        [FlagArgument.Arguments] = AddCommandArguments,
        [FlagArgument.ConsoleType] = AddCommandConsoleType,
        [FlagArgument.Description] = AddCommandDescription
    };

    public override Result TryBind(string[] inlineArgs)
    {
        Logger.Info("binding");
        switch (inlineArgs.Length)
        {
            case 0:
                return "Command name is missing.";
            case > 1:
                return "Command name can only be a single word, no whitespace allowed.";
        }
        
        var name = inlineArgs.First();
        if (name.Any(char.IsWhiteSpace))
        {
            return "Command name can only be a single word, no whitespace allowed.";
        }
        
        Command = new CustomCommand
        {
            Command = name
        };
        
        return true;
    }

    public override void Confirm()
    {
        ScriptCommands.Add(Command, this);
        
        foreach (var console in Command.ConsoleTypes.GetFlags())
        {
            switch (console)
            {
                case ConsoleType.Player:
                    QueryProcessor.DotCommandHandler.RegisterCommand(Command);
                    continue;
                case ConsoleType.Server:
                    GameCore.Console.singleton.ConsoleCommandHandler.RegisterCommand(Command);
                    continue;
                case ConsoleType.RemoteAdmin:
                    CommandProcessor.RemoteAdminCommandHandler.RegisterCommand(Command);
                    continue;
                case ConsoleType.None:
                    continue;
                default:
                    throw new AndrzejFuckedUpException();
            }
        }
    }

    public override void Unbind()
    {
        ScriptCommands.Remove(Command);
        
        foreach (var console in Command.ConsoleTypes.GetFlags())
        {
            switch (console)
            {
                case ConsoleType.Player:
                    QueryProcessor.DotCommandHandler.UnregisterCommand(Command);
                    break;
                case ConsoleType.Server:
                    GameCore.Console.singleton.ConsoleCommandHandler.UnregisterCommand(Command);
                    break;
                case ConsoleType.RemoteAdmin:
                    CommandProcessor.RemoteAdminCommandHandler.UnregisterCommand(Command);
                    break;
                case ConsoleType.None:
                    continue;
                default:
                    throw new AndrzejFuckedUpException();
            }
        }
    }
    
    public class CustomCommand : ICommand, IUsageProvider, IHelpProvider
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (RunAttachedScript(this, ScriptExecutor.Get(sender, arguments), arguments.ToArray()).HasErrored(out var error))
            {
                response = error;
                return false;       
            }
        
            response = "Command executed.";
            return true;
        }

        public required string Command { get; init; }
        public string[] Aliases { get; set; } = [];
        public string Description { get; set; } = "";
        public ConsoleType ConsoleTypes { get; set; } = ConsoleType.Server;
        public string[] Usage { get; set; } = [];
        public string GetHelp(ArraySegment<string> arguments)
        {
            Logger.Info($"arguments: {arguments.JoinStrings(" ")}, usage: {Usage.JoinStrings(" ")}");
            return $"Description: {Description}\n" +
                   $"Arguments: {Usage.Select(arg => $"[{arg}]").JoinStrings(" ")}";
        }
    }

    public static readonly Dictionary<CustomCommand, CommandFlag> ScriptCommands = [];

    public CustomCommand Command = null!;

    public static Result RunAttachedScript(CustomCommand requestingCommand, ScriptExecutor sender, string[] args)
    {
        if (!ScriptCommands.TryGetValue(requestingCommand, out var flag))
        {
            return "The script that was supposed to handle this command was not found.";
        }

        if (args.Length < requestingCommand.Usage.Length)
        {
            return "Not enough arguments. " +
                   $"Expected {requestingCommand.Usage.Length} but got {args.Length}. " +
                   $"Usage: {requestingCommand.GetHelp(new ArraySegment<string>(args))}";
        }

        if (args.Length > requestingCommand.Usage.Length)
        {
            return "Too many arguments. " +
                   $"Expected {requestingCommand.Usage.Length} but got {args.Length}. " +
                   $"Usage: {requestingCommand.GetHelp(new ArraySegment<string>(args))}";
        }

        if (Script.CreateByScriptName(flag.ScriptName, sender)
            .HasErrored(out var error, out var script))
        {
            return error;
        }

        switch (sender)
        {
            case PlayerConsoleExecutor playerConsole:
                script.AddLocalPlayerVariable(new("sender", [Player.Get(playerConsole.Sender)]));
                break;
            case RemoteAdminExecutor remoteAdminExecutor:
                script.AddLocalPlayerVariable(new("sender", [Player.Get(remoteAdminExecutor.Sender)]));
                break;
        }

        for (var index = 0; index < requestingCommand.Usage.Length; index++)
        {
            var argVariable = requestingCommand.Usage[index];
            var name = argVariable[0].ToString().ToLower() + argVariable.Substring(1);
            var value = args[index];
            
            script.AddLocalLiteralVariable(new()
            {
                Name = name,
                Value = () => value
            });
        }

        script.Run();
        return true;
    }
    
    private Result AddCommandArguments(string[] args)
    {
        foreach (var arg in args)
        {
            if (!arg.All(char.IsLetter))
            {
                return $"Argument '{arg}' contains non-letter characters.";
            }
        }

        Command.Usage = args;
        return true;
    }

    private Result AddCommandConsoleType(string[] args)
    {
        ConsoleType types = ConsoleType.None;

        foreach (var arg in args)
        {
            if (Enum.TryParse(arg, true, out ConsoleType consoleType))
            {
                types |= consoleType;
                continue;
            }

            return $"Value '{arg}' is not a valid {nameof(ConsoleType)}";
        }
        
        Command.ConsoleTypes = types;
        return true;
    }

    private Result AddCommandDescription(string[] args)
    {
        Command.Description = args.JoinStrings(" ");
        return true;
    }
}