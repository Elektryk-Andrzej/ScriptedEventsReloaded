using LabApi.Features.Wrappers;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.RoundMethods;

public class RoundLockMethod : SynchronousMethod
{
    public override string Description => "Changes the round lock state.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new BoolArgument("new state")
    ];
    
    public override void Execute()
    {
        Round.IsLocked = Args.GetBool("new state");
    }
}