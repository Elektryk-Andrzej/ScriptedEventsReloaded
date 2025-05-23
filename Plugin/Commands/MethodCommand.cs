using System;
using System.Linq;
using CommandSystem;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using SER.ScriptSystem;

namespace SER.Plugin.Commands;

[CommandHandler(typeof(GameConsoleCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class MethodCommand : ICommand
{
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var player = Player.Get(sender);
        if (player is not null && player.HasPermissions("ser.run"))
        {
            response = "You do not have permission to run scripts.";
            return false;
        }
        
        new Script
        {
            Name = "Command",
            Content = string.Join(" ", arguments.ToArray())
        }.Run();
        
        response = "Method executed.";
        return true;
    }
    

    public string Command => "sermethod";
    public string[] Aliases => [];
    public string Description => string.Empty;
}