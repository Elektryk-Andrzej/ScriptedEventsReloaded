﻿using PlayerRoles;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.PlayerMethods;

public class SetRoleMethod : Method
{
    public override string Description => "Sets a role for players.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("players"),
        new EnumArgument<RoleTypeId>("newRole"),
        new EnumArgument<RoleSpawnFlags>("spawnFlags")
        {
            DefaultValue = RoleSpawnFlags.All
        },
        new EnumArgument<RoleChangeReason>("reason")
        {
            DefaultValue = RoleChangeReason.RemoteAdmin
        }
    ];

    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var newRole = Args.GetEnum<RoleTypeId>("newRole");
        var reason = Args.GetEnum<RoleChangeReason>("reason");
        var flags = Args.GetEnum<RoleSpawnFlags>("spawnFlags");
        
        foreach (var player in players) 
            player.SetRole(newRole, reason, flags);
    }
}