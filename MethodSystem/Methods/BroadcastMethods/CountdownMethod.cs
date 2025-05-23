using System;
using System.Collections.Generic;
using LabApi.Features.Wrappers;
using MEC;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.BroadcastMethods;

public class CountdownMethod : Method
{
    public override string Description => "Creates a countdown using broadcasts.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("players"),
        new DurationArgument("duration"),
        new TextArgument("title")
        {
            Description = "Use %seconds% to get seconds remaining until the end of the countdown."
        }
    ];
    
    public override void Execute()
    {
        RunCoroutine(Countdown());
    }

    private IEnumerator<float> Countdown()
    {
        var players = Args.GetPlayers("players");
        var duration = Args.GetDuration("duration");
        var title = Args.GetText("title");

        while (duration.TotalSeconds > 0)
        {
            var isLastCycle = duration.TotalSeconds <= 1;
            var secondsRemaining = Math.Round(duration.TotalSeconds, MidpointRounding.AwayFromZero);
            var currentTitle = title.Replace("%seconds%", secondsRemaining.ToString());

            foreach (var plr in players)
            {
                plr.ClearBroadcasts();
                Server.SendBroadcast(plr, currentTitle, (ushort)(isLastCycle ? 1 : 2));
            }
            
            duration -= TimeSpan.FromSeconds(1);
            yield return Timing.WaitForSeconds(1);
        }
    }
}