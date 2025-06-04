using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CommandSystem;

namespace SER.ScriptSystem.FlagSystem.Commands;

public class CustomCommand : ICommand, IUsageProvider
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        var boundScript = ScriptFlagHandler.CommandBoundScripts.FirstOrDefault(x => x.Command == this);
        if (boundScript is null)
        {
            response = "The script that was supposed to handle this command was not found.";
            return false;
        }

        if (Script.CreateByScriptName(boundScript.ScriptName).HasErrored(out var error, out var script))
        {
            response = error;
            return false;
        }
        
        script.Run();
        response = "Command executed.";
        return true;
    }

    public required string Command { get; init; }
    public string[] Aliases { get; set; } = [];
    public string Description { get; set; } = "";
    public ConsoleType[] ConsoleTypes { get; set; } = [ConsoleType.Server];
    public string[] Usage { get; set; } = [];
}