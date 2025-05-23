using System;
using System.Linq;
using CommandSystem;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using SER.ScriptSystem;

namespace SER.Plugin.Commands;

[CommandHandler(typeof(GameConsoleCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class RunScriptCommand : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var player = Player.Get(sender);
        if (player is not null && player.HasPermissions("ser.run"))
        {
            response = "You do not have permission to run scripts.";
            return false;
        }
        
        var name = arguments.FirstOrDefault();
        
        if (name is null)
        {
            response = "No script name provided.";
            return false;
        }

        if (Script.CreateByScriptName(name).HasErrored(out var err, out var script))
        {
            response = err;
            return false;
        }
        
        script.Run();
        response = "Script is now running!";
        return true;
    }

    public string Command => "serrun";
    public string[] Aliases => [];
    public string Description => string.Empty;
}