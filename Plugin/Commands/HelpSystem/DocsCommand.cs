﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using CommandSystem;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using SER.Helpers.Exceptions;
using SER.Plugin.Commands.Interfaces;

namespace SER.Plugin.Commands.HelpSystem;

[CommandHandler(typeof(GameConsoleCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class DocsCommand : ICommand, IUsePermissions
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var player = Player.Get(sender);
        if (player is not null && player.HasPermissions(Permission))
        {
            response = "You do not have permission to create documentation.";
            return false;
        }

        var sb = new StringBuilder($"Genrated on [{DateTime.Today.ToLongDateString()}] with SER version [{MainPlugin.Instance.Version}]\n\n");

        var helpOptions = Enum.GetValues(typeof(HelpOption)).Cast<HelpOption>();
        foreach (var helpOption in helpOptions)
        {
            if (!HelpCommand.GeneralOptions.TryGetValue(helpOption, out var generalOption))
            {
                throw new AndrzejFuckedUpException(
                    $"Option {helpOption} is not registered in the help command.");
            }
            
            sb.AppendLine($"===== {helpOption} =====");
            sb.AppendLine(generalOption());
            sb.AppendLine();
        }

        using (var sw = File.CreateText(Path.Combine(FileSystem.DirPath, "# Documentation #.txt")))
        {
            sw.Write(sb.ToString());
            sw.Flush();
            sw.Close();
        }
        
        response = "Documentation successfully generated! Check the file '# Documentation #.txt' in the SER folder.";
        return true;
    }

    public string Command => "serdocs";
    public string[] Aliases => [];
    public string Description => "Generates documentation for the plugin.";
    public string Permission => "ser.docs";
}