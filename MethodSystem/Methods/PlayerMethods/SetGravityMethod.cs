using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.PlayerMethods;

public class SetGravityMethod : SynchronousMethod
{
    public override string Description => "Changes player gravity.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("players"),
        new FloatArgument("gravity")
    ];
    
    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var gravity = Args.GetFloat("gravity");
        
        players.ForEach(plr => plr.Gravity = new(0, -gravity, 0));
    }
}