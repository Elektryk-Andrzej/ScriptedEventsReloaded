using System;
using System.Linq;
using CommandSystem;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using SER.Plugin.Commands.Interfaces;
using SER.ScriptSystem;
using SER.ScriptSystem.Structures;

namespace SER.Plugin.Commands;

[CommandHandler(typeof(GameConsoleCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class MethodCommand : ICommand, IUsePermissions
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var player = Player.Get(sender);
        if (player is not null && player.HasPermissions(Permission))
        {
            response = "You do not have permission to run scripts.";
            return false;
        }

        var script = new Script
        {
            Name = "Command",
            Content = string.Join(" ", arguments.ToArray()),
            Executor = ScriptExecutor.Get(sender, arguments)
        };
        
        script.Run();
        response = "Method executed.";
        return true;
    }
    
    public string Command => "sermethod";
    public string[] Aliases => [];
    public string Description => "Runs the provied arguments at it was a line in a script.";
    public string Permission => "ser.run";
}