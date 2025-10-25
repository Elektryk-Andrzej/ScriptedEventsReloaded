using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.PlayerMethods;

public class SetSizeMethod : SynchronousMethod
{
    public override string Description => "Sets the size of players.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("players"),
        new FloatArgument("x size"),
        new FloatArgument("y size"),
        new FloatArgument("z size")
    ];
    
    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var x = Args.GetFloat("x size");
        var y = Args.GetFloat("y size");
        var z = Args.GetFloat("z size");
        
        players.ForEach(plr =>
        {
            plr.Scale = new(x, y, z);
        });
    }
}