using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.BroadcastMethods;

public class ClearBroadcastsMethod : SynchronousMethod
{
    public override string Description => "Clears broadcasts for players.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("players")
    ];
    
    public override void Execute()
    {
        var players = Args.GetPlayers("players");

        foreach (var plr in players)
        {
            plr.ClearBroadcasts();
        }
    }
}