using System;
using LabApi.Features.Wrappers;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.Exceptions;
using SER.MethodSystem.BaseMethods;
using SER.ValueSystem;

namespace SER.MethodSystem.Methods.RoundMethods;

public class RoundInfoMethod : LiteralValueReturningMethod
{
    public override string Description => "Returns information about the current round.";
    
    public override Type[] ReturnTypes => [typeof(BoolValue)];

    public override Argument[] ExpectedArguments { get; } =
    [
        new OptionsArgument("mode",
            "hasStarted",
            "isInProgress",
            "hasEnded")
    ];

    public override void Execute()
    {
        ReturnValue = Args.GetOption("mode") switch
        {
            "hasstarted" => new BoolValue(Round.IsRoundStarted),
            "isinprogress" => new BoolValue(Round.IsRoundInProgress),
            "hasended" => new BoolValue(Round.IsRoundEnded),
            _ => throw new AndrzejFuckedUpException()
        };
    }
}