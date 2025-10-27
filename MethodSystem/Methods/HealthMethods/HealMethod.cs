using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.HealthMethods;

public class HealMethod : SynchronousMethod
{
    public override string Description => "Heals players. Doesn't exceed their max health.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("players to heal"),
        new FloatArgument("amount", 0)
        {
            DefaultValue = float.MaxValue,
            Description = "If this argument is not provided, all players will be fully healed."
        }  
    ];
    
    public override void Execute()
    {
        var amount = Args.GetFloat("amount");
        Args.GetPlayers("players to heal").ForEach(plr =>
        {
            plr.Heal(amount);
        });
    }
}