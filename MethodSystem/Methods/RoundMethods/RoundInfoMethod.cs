using LabApi.Features.Wrappers;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.Exceptions;
using SER.MethodSystem.MethodDescriptors;

namespace SER.MethodSystem.Methods.RoundMethods;

public class RoundInfoMethod : TextReturningMethod, IPureMethod
{
    public override string Description => "Returns information about the current round.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
        new OptionsArgument("mode",
            "hasStarted",
            "isInProgress",
            "hasEnded")
    ];

    public override void Execute()
    {
        TextReturn = Args.GetOption("mode") switch
        {
            "hasstarted" => Round.IsRoundStarted.ToString(),
            "isinprogress" => Round.IsRoundInProgress.ToString(),
            "hasended" => Round.IsRoundEnded.ToString(),
            _ => throw new DeveloperFuckupException()
        };
    }
}