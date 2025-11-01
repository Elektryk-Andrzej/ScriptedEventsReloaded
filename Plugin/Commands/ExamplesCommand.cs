using System;
using CommandSystem;

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