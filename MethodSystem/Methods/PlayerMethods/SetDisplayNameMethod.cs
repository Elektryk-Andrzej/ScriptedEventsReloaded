using LabApi.Features.Wrappers;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using System.Collections.Generic;

namespace SER.MethodSystem.Methods.PlayerMethods;
public class SetDisplayNameMethod : SynchronousMethod
{
    public override string Description => "Sets display name for specified players";

    public override Argument[] ExpectedArguments =>
    [
        new PlayersArgument("players"),
        new TextArgument("displayname")
        {
            Description = "Leave empty quotes if you want to remove display name from players"
        }
    ];

    public override void Execute()
    {
        string disp = Args.GetText("displayname");
        List<Player> pls = Args.GetPlayers("players");
        foreach(Player p in pls)
        {
            p.DisplayName = disp;
        }
    }
}
