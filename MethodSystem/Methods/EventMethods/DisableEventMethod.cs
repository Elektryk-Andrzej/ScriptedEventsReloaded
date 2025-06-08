using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;
using SER.ScriptSystem.EventSystem;

namespace SER.MethodSystem.Methods.EventMethods;

public class DisableEventMethod : Method
{
    public override string Description => "Disables the provided event from running.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
        new TextArgument("eventName")
    ];
    
    public override void Execute()
    {
        EventHandler.DisableEvent(Args.GetText("eventName"), Script.Name);
    }
}