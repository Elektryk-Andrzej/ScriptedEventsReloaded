using System;
using CommandSystem;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using SER.Plugin.Commands.Interfaces;
using SER.ScriptSystem;

namespace SER.Plugin.Commands;

[CommandHandler(typeof(GameConsoleCommandHandler))]
public class ExamplesCommand : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        FileSystem.GenerateExamples();
        response = "Examples have been generated!";
        return true;
    }

    public string Command => "serexamples";
    public string[] Aliases => [];
    public string Description => "Generates example scripts.";
}