using Respawning;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.CASSIEMethods;

public class CassieMethod : SynchronousMethod
{
    public override string Description => "Makes a CASSIE announcement.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new OptionsArgument("mode",
            "jingle",
            "noJingle"
        ),
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