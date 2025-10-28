using LabApi.Features.Wrappers;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.RoundMethods;

public class EndRoundMethod : SynchronousMethod
{
    public override string? Description => "Ends a round.";

    public override Argument[] ExpectedArguments { get; } = [];
    
    public override void Execute()
    {
        Round.End(true);
    }
}