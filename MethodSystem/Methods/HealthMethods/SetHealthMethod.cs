using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.HealthMethods;

public class SetHealthMethod : SynchronousMethod
{
    public override string Description => "Sets health for players.";
    public override Argument[] ExpectedArguments { get; } = 
    [
        new PlayersArgument("players"),
        new FloatArgument("health", 0)
    ];
    
    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var health = Args.GetFloat("health");
        foreach (var player in players) player.Health = health;
    }
}