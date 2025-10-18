using LabApi.Features.Wrappers;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.RoundMethods;

public class LobbyLockMethod : SynchronousMethod
{
    public override string Description => "Changes the lobby lock state.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new BoolArgument("new state")
    ];
    
    public override void Execute()
    {
        Round.IsLobbyLocked = Args.GetBool("new state");
    }
}