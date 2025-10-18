﻿using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.PlayerMethods;

public class BanMethod : SynchronousMethod
{
    public override string Description => "Bans players from the server.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("players"),
        new DurationArgument("duration"),
        new TextArgument("reason")
    ];
    
    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var duration = Args.GetDuration("duration");
        var reason = Args.GetText("reason");
        
        players.ForEach(p => p.Ban(reason, (long)duration.TotalSeconds));
    }
}