using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.HealthMethods;

public class KillMethod : SynchronousMethod
{
    public override string Description => "Kills players.";

    public override GenericMethodArgument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("players"),
    ];
    
    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        
        foreach (var player in players)
        {
            player.Kill();
        }
    }
}