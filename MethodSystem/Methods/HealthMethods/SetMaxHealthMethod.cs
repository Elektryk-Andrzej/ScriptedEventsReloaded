using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.HealthMethods;

public class SetMaxHealthMethod : SynchronousMethod
{
    public override string Description => "Sets the max health of players.";

    public override GenericMethodArgument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("players"),
        new FloatArgument("maxHealth", 1)
    ];
    
    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var maxHealth = Args.GetFloatAmount("maxHealth");
        
        foreach (var player in players) player.MaxHealth = maxHealth;
    }
}