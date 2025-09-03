using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.PlayerMethods;

public class GodmodeMethod : SynchronousMethod
{
    public override string Description => "Enables or disables godmode for specified players.";

    public override GenericMethodArgument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("players"),
        new BoolArgument("mode")
    ];

    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var mode = Args.GetBool("mode");

        players.ForEach(p => p.IsGodModeEnabled = mode);
    }
}