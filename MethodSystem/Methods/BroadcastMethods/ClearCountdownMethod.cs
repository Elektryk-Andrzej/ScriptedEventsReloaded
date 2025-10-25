﻿using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.BroadcastMethods;

public class ClearCountdownMethod : SynchronousMethod
{
    public override string Description => "Removes an active countdown for players if one is active.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("players")
    ];
    
    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        players.ForEach(plr =>
        {
            if (CountdownMethod.Coroutines.TryGetValue(plr, out var coroutine)) 
                coroutine.Kill();
        });
    }
}