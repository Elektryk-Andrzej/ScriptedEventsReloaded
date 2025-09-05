using LabApi.Features.Wrappers;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using System.Collections.Generic;

namespace SER.MethodSystem.Methods.PlayerMethods;
public class SetGroupMethod : SynchronousMethod
{
    public override string Description => "Sets or removes group for provided players";

    public override GenericMethodArgument[] ExpectedArguments =>
    [
        new PlayersArgument("players"),
        new TextArgument("group")
        {
            Description = "Set to NONE if you want to remove the group!",
        }
    ];

    public override void Execute()
    {
        List<Player> pls = Args.GetPlayers("players");
        string group = Args.GetText("group");
        pls.ForEach(p => p.UserGroup = ServerStatic.PermissionsHandler.GetGroup(group != "NONE" ? group : null));
    }
}