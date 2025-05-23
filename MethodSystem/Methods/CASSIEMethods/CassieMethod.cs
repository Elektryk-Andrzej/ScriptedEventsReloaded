using Respawning;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.CASSIEMethods;

public class CassieMethod : Method
{
    public override string Description => "Makes a CASSIE announcement.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
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
        var isNoisy = Args.GetOption("mode") == "jingle";
        var message = Args.GetText("message");
        var translation = Args.GetText("translation");
        
        RespawnEffectsController.PlayCassieAnnouncement(
            message, 
            false, 
            isNoisy, 
            !string.IsNullOrWhiteSpace(translation),
            translation);
    }
}