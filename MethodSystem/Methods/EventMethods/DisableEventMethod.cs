using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.EventSystem;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.EventMethods;

public class DisableEventMethod : SynchronousMethod
{
    public override string Description => "Disables the provided event from running.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new TextArgument("eventName")
    ];
    
    public override void Execute()
    {
        EventHandler.DisableEvent(Args.GetText("eventName"), Script.Name);
    }
}