using System;
using CommandSystem;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using SER.ScriptSystem.FlagSystem;

namespace SER.Plugin.Commands;

[CommandHandler(typeof(GameConsoleCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class ReloadScriptsCommand : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var player = Player.Get(sender);
        if (player is not null && player.HasPermissions("ser.reload"))
        {
            response = "You do not have permission to reload scripts.";
            return false;
        }
        
        ScriptFlagHandler.Clear();
        FileSystem.Initalize();
        
        response = "Successfully reloaded scripts. Changes in script flags are now registered.";
        return true;
    }

    public string Command => "serreload";
    public string[] Aliases => [];
    public string Description => string.Empty;
}