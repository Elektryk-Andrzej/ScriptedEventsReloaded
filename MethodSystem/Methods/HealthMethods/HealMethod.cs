using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.HealthMethods;

public class HealMethod : SynchronousMethod
{
    public override string Description => "Heals players.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("players to heal"),
        new FloatArgument("amount", 0)   
    ];
    
    public override void Execute()
    {
        Args.GetPlayers("players to heal").ForEach(plr => plr.Health += Args.GetFloat("amount"));
    }
}