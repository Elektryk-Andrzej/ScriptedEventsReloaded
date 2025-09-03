﻿using LabApi.Features.Wrappers;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.BroadcastMethods;

public class BroadcastMethod : SynchronousMethod
{
    public override string Description => "Sends a broadcast to players.";

    public override GenericMethodArgument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("players"),
        new DurationArgument("duration"),
        new TextArgument("message")
    ];

    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var duration = Args.GetDuration("duration");
        var message = Args.GetText("message");

        foreach (var player in players) 
            Server.SendBroadcast(player, message, (ushort)duration.TotalSeconds);
    }
}