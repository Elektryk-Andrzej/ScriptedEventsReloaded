using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.PlayerMethods;

public class KickMethod : SynchronousMethod
{
    public override string Description => "Kicks players from the server.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("players"),
        new TextArgument("reason")
    ];
    
    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var reason = Args.GetText("reason");
        
        players.ForEach(p => p.Kick(reason));
    }
}