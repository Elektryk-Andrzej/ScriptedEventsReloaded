using LabApi.Features.Wrappers;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using System.Collections.Generic;

namespace SER.MethodSystem.Methods.PlayerMethods;
public class SetInfoAreaMethod : SynchronousMethod
{
    public override string Description => "Sets InfoArea for specified players";

    public override Argument[] ExpectedArguments => [
        new PlayersArgument("players"),
        new EnumArgument<PlayerInfoArea>("infoarea")
        {
            ConsumesRemainingValues = true,
        }
        ];

    public override void Execute()
    {
        List<Player> pls = Args.GetPlayers("players");
        PlayerInfoArea area = Args.GetEnum<PlayerInfoArea>("infoarea");
        foreach(Player p in pls)
        {
            p.InfoArea = area;
        }
    }
}
