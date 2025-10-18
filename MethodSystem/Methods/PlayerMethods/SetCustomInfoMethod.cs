using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.PlayerMethods;

public class SetCustomInfoMethod : SynchronousMethod
{
    public override string Description =>
        "Sets custom info (overhead text) for specific players, visible to specific target players.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("playersToAffect"),
        new TextArgument("customInfoText"),
    ];

    public override void Execute()
    {
        var players = Args.GetPlayers("playersToAffect");
        var text = Args.GetText("customInfoText")
            .Replace("\\n", "\n")
            .Replace("<br>", "\n");

        foreach (var player in players)
        {
            player.CustomInfo = text;
        }
    }
}