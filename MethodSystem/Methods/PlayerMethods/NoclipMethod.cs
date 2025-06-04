using PlayerRoles.FirstPersonControl;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.PlayerMethods;

public class NoclipMethod : Method
{
    public override string Description => "Enables or disables access to noclip for specified players.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("players"),
        new BoolArgument("isAllowed")
    ];

    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var isAllowed = Args.GetBool("isAllowed");
        
        foreach (var player in players)
        {
            if (isAllowed)
            {
                FpcNoclip.PermitPlayer(player.ReferenceHub);
            }
            else
            {
                FpcNoclip.UnpermitPlayer(player.ReferenceHub);
            }
        }
    }
}