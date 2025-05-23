using System;
using PlayerRoles;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.MethodDescriptors;

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
            "role",
            "team",
            new("serverId", "The ID assigned by the server, used mostly for commands."),
            new("userId", "ID attached to the account, like 1233455@steam"))
    ];

    public override void Execute()
    {
        var plr = Args.GetSinglePlayer("player");
        TextReturn = Args.GetOption("property").ToLower() switch
        {
            "name" => plr.Nickname,
            "role" => plr.Role.ToString(),
            "team" => plr.Role.GetTeam().ToString(),
            "displayname" => plr.DisplayName,
            "serverid" => plr.PlayerId.ToString(),
            "userid" => plr.UserId,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}