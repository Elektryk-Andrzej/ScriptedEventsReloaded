using System.Collections.Generic;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using LabApi.Features.Wrappers;

namespace SER.MethodSystem.Methods.PlayerMethods;

public class SetGroupMethod : SynchronousMethod
{
    public override string Description => "Sets or removes group from specified players";
    
    public override Argument[] ExpectedArguments => [
        new PlayersArgument("players"),
        new TextArgument("group"){Description = "Name of the group or set to NONE if you want to remove group from specified players" }
    ];

    public override void Execute()
    {
        string gr = Args.GetText("group");
        List<Player> pls = Args.GetPlayers("players");
        pls.ForEach(p=>p.UserGroup = ServerStatic.PermissionsHandler.GetGroup(gr == "NONE" ? null: gr));
    }
}