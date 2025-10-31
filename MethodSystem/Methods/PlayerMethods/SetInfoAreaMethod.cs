using LabApi.Features.Wrappers;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using System.Collections.Generic;

namespace SER.MethodSystem.Methods.PlayerMethods;

public class SetInfoAreaMethod : SynchronousMethod
{
    public override string Description => "Sets InfoArea for specified players";

    public override Argument[] ExpectedArguments => 
    [
        new PlayersArgument("players"),
        new EnumArgument<PlayerInfoArea>("info area")
        {
            ConsumesRemainingValues = true,
        }
    ];

    public override void Execute()
    {
        List<Player> players = Args.GetPlayers("players");
        PlayerInfoArea info = Args.GetEnum<PlayerInfoArea>("info area");
        
        players.ForEach(p => p.InfoArea = info);
    }
}
