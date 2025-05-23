using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.HealthMethods;

public class KillMethod : Method
{
    public override string Description => "Kills players.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
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