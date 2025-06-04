using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.PlayerMethods;

public class SetCustomInfoMethod : Method
{
    public override string Description =>
        "Sets custom info (overhead text) for specific players, visible to specific target players.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
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