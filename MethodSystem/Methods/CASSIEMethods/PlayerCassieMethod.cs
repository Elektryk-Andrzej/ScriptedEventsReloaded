namespace SER.MethodSystem.Methods.CASSIEMethods;

/*
public class PlayerCassieMethod : Method
{
    public override string Description => "Makes a CASSIE announcement to specified players only.";

    public override GenericMethodArgument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("players"),
        new OptionsArgument("mode",
            "jingle",
            "noJingle"),
        new TextArgument("message"),
        new TextArgument("translation")
        {
            DefaultValue = "",
        }
    ];

    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var isNoisy = Args.GetOption("mode") == "jingle";
        var message = Args.GetText("message");
        var translation = Args.GetText("translation");

        foreach (var player in players)
        {
            player.MessageTranslated(message, translation, false, isNoisy, !string.IsNullOrWhiteSpace(translation));
        }
    }
}
*/