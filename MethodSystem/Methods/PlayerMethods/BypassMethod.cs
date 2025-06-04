using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.PlayerMethods;

public class BypassMethod : Method
{
    public override string Description => "Grants or removes bypass mode for players.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("players"),
        new BoolArgument("mode")
    ];

    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var mode = Args.GetBool("mode");

        players.ForEach(p => p.IsBypassEnabled = mode);
    }
}