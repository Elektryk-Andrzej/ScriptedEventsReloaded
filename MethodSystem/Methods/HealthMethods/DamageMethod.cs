using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.HealthMethods;

public class DamageMethod : SynchronousMethod
{
    public override string Description => "Damages players.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("players"),
        new FloatArgument("amount", 0),
        new TextArgument("reason")
        {
            DefaultValue = string.Empty
        }
    ];
    
    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var amount = Args.GetFloat("amount");
        var reason = Args.GetText("reason");
        
        foreach (var plr in players)
        {
            plr.Damage(amount, reason);
        }
    }
}