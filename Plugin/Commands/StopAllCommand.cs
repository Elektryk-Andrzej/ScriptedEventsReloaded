﻿using System;
using CommandSystem;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using SER.Plugin.Commands.Interfaces;
using SER.ScriptSystem;

namespace SER.Plugin.Commands;

[CommandHandler(typeof(GameConsoleCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class StopAllCommand : ICommand, IUsePermissions
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var player = Player.Get(sender);
        if (player is not null && player.HasPermissions(Permission))
        {
            response = "You do not have permission to stop scripts.";
            return false;
        }
        
        response = $"Stopped {Script.StopAll()} scripts.";
        return true;
    }

    public string Command => "serstopall";
    public string[] Aliases => [];
    public string Description => "Stops all running scripts.";
    public string Permission => "ser.stop";
}