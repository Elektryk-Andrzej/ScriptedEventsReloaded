using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;
using Utils;

namespace SER.MethodSystem.Methods.PlayerMethods;

public class ExplodeMethod : Method
{
    public override string Description => "Explodes players.";
    
    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("players")
    ];

    public override void Execute()
    {
        Args.GetPlayers("players").ForEach(player => 
            ExplosionUtils.ServerExplode(player.ReferenceHub, ExplosionType.Grenade));
    }
}