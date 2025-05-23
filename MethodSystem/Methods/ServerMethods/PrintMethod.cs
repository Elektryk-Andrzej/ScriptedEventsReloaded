using System;
using LabApi.Features.Console;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.ServerMethods;

public class PrintMethod : Method
{
    public override string Description => "Prints the text provided to the server console.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
        new TextArgument("text"),
        new EnumArgument<ConsoleColor>("color")
        {
            DefaultValue = ConsoleColor.Cyan
        }
    ];

    public override void Execute()
    {
        Logger.Raw($"[Script '{Script.Name}'] {Args.GetText("text")}", Args.GetEnum<ConsoleColor>("color"));
    }
}