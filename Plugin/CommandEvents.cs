using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Enums;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using SER.Helpers.Extensions;
using SER.Plugin.Commands;
using SER.ScriptSystem;
using SER.ScriptSystem.Structures;
using SER.TokenSystem;
using SER.TokenSystem.Tokens;

namespace SER.Plugin;

public static class CommandEvents
{
    public static readonly Dictionary<ICommandSender, CommandType> UsedCommandTypes = [];
    
    public static void Initialize()
    {
        UsedCommandTypes.Clear();
        ServerEvents.CommandExecuting += CaptureComamnd;
    }

    public static void CaptureComamnd(CommandExecutingEventArgs ev)
    {
        UsedCommandTypes[ev.Sender] = ev.CommandType;
        
        if (MainPlugin.Instance.Config?.SerMethodsAsCommands != true)
        {
            return;
        }
        
        if (Player.Get(ev.Sender) is { } player && player.HasPermissions(MethodCommand.RunPermission))
        {
            return ;
        }

        if (!ev.CommandName.StartsWith(">"))
        {
            return;
        }
        
        var methodName = ev.CommandName.Substring(1);
        
        if (!methodName.Any())
        {
            return;
        }
        
        var script = new Script
        {
            Name = methodName,
            Content = $"{methodName} {ev.Arguments.JoinStrings(" ")}",
            Executor = ScriptExecutor.Get(ev.Sender, ev.CommandType)
        };

        if (Tokenizer.SliceLine(methodName).HasErrored(out _, out var slices))
        {
            return;
        }
        
        // check if the a method like this exists
        var instance = new MethodToken();
        var res = (BaseToken.IParseResult) typeof(MethodToken)
            .GetMethod(nameof(MethodToken.TryInit))!
            .Invoke(instance, [slices.First(), script, null]);

        if (res is not BaseToken.Success)
        {
            return;
        }

        ev.IsAllowed = false;
        script.Run();
    }
}