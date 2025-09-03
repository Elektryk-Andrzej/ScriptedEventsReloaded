using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using Utils;

namespace SER.MethodSystem.Methods.PlayerMethods;

public class ExplodeMethod : SynchronousMethod
{
    public override string Description => "Explodes players.";
    
    public override GenericMethodArgument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("players")
    ];

    public override void Execute()
    {
        Args.GetPlayers("players").ForEach(player => 
            ExplosionUtils.ServerExplode(player.ReferenceHub, ExplosionType.Grenade));
    }
}