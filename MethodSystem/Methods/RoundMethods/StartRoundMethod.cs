using LabApi.Features.Wrappers;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.RoundMethods;

public class StartRoundMethod : SynchronousMethod
{
    public override string Description => "Start a round.";

    public override Argument[] ExpectedArguments { get; } = [];
    
    public override void Execute()
    {
        Round.Start();
    }
}