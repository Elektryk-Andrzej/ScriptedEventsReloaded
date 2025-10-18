using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.HealthMethods;

public class SetMaxHealthMethod : SynchronousMethod
{
    public override string Description => "Sets the max health of players.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("players"),
        new FloatArgument("maxHealth", 1)
    ];
    
    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var maxHealth = Args.GetFloat("maxHealth");
        
        foreach (var player in players) player.MaxHealth = maxHealth;
    }
}