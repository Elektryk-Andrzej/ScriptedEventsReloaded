using System;
using CommandSystem;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using SER.Plugin.Commands.Interfaces;
using SER.ScriptSystem.FlagSystem;

namespace SER.Plugin.Commands;

[CommandHandler(typeof(GameConsoleCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class ReloadCommand : ICommand, IUsePermissions
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var player = Player.Get(sender);
        if (player is not null && player.HasPermissions(Permission))
        {
            response = "You do not have permission to reload scripts.";
            return false;
        }
        
        ScriptFlagHandler.Clear();
        FileSystem.Initialize();
        
        response = "Successfully reloaded scripts. Changes in script flags are now registered.";
        return true;
    }

    public string Command => "serreload";
    public string[] Aliases => [];
    public string Description => 
        "Reloads all scripts. Use when you add/remove flags in a script for them to be registered if the server is running.";

    public string Permission => "ser.reload";
}