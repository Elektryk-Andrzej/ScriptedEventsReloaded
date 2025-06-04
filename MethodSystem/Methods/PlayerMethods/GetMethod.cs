using System;
using LabApi.Features.Wrappers;
using MapGeneration;
using PlayerRoles;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.MethodDescriptors;
using SER.VariableSystem;

namespace SER.MethodSystem.Methods.PlayerMethods;

public class GetMethod : TextReturningMethod, IPureMethod
{
    public override string Description => "Returns the requested properties about the player.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
        new SinglePlayerArgument("player"),
        new OptionsArgument("property",
            "name",
            "displayName",
            new("role", $"{nameof(RoleTypeId)} enum value"),
            new("team", $"{nameof(Team)} enum value"),
            new("serverId", "ID assigned by the server, usually used for commands e.g. 2"),
            new("userId", "ID attached to the account e.g. 0123456789@steam"),
            new("room", $"{nameof(Room)} reference or UNDEFINED if not in a room"),
            new("zone", $"{nameof(FacilityZone)} enum value"))
    ];

    public override void Execute()
    {
        var plr = Args.GetSinglePlayer("player");
        TextReturn = Args.GetOption("property").ToLower() switch
        {
            "name" => plr.Nickname,
            "displayname" => plr.DisplayName,
            "role" => plr.Role.ToString(),
            "team" => plr.Role.GetTeam().ToString(),
            "serverid" => plr.PlayerId.ToString(),
            "userid" => plr.UserId,
            "room" => plr.Room is not null 
                ? ObjectReferenceSystem.RegisterObject(plr.Room)
                : "UNDEFINED",
            "zone" => plr.Zone.ToString(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}