using System;
using System.Diagnostics.CodeAnalysis;
using CommandSystem;
using SER.Testing;

namespace SER.Plugin.Commands;

[CommandHandler(typeof(GameConsoleCommandHandler))]
public class TestCommand : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        if (Test.Run().HasErrored(out var error))
        {
            response = error;
            return false;
        }

        response = "All tests passed - SER is working as intended!";
        return true;
    }

    public string Command => "sertest";
    public string[] Aliases => [];
    public string Description => string.Empty;
}