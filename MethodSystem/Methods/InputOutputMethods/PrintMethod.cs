using System;
using LabApi.Features.Console;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.InputOutputMethods;

public class PrintMethod : SynchronousMethod
{
    public override string Description => "Prints the text provided to the server console.";

    public override GenericMethodArgument[] ExpectedArguments { get; } =
    [
        new TextArgument("text"),
        new EnumArgument<ConsoleColor>("color")
        {
            DefaultValue = ConsoleColor.Green
        }
    ];

    public override void Execute()
    {
        Logger.Raw(Args.GetText("text"), Args.GetEnum<ConsoleColor>("color"));
    }
}