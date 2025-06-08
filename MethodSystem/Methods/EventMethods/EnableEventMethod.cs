using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;
using SER.ScriptSystem.EventSystem;

namespace SER.MethodSystem.Methods.EventMethods;

public class EnableEventMethod : Method
{
    public override string Description => "Enables the provided event to run after being disabled.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
        new TextArgument("eventName")
    ];
    
    public override void Execute()
    {
        EventHandler.EnableEvent(Args.GetText("eventName"));
    }
}