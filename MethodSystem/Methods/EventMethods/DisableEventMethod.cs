using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using SER.ScriptSystem.EventSystem;

namespace SER.MethodSystem.Methods.EventMethods;

public class DisableEventMethod : SynchronousMethod
{
    public override string Description => "Disables the provided event from running.";

    public override GenericMethodArgument[] ExpectedArguments { get; } =
    [
        new TextArgument("eventName")
    ];
    
    public override void Execute()
    {
        EventHandler.DisableEvent(Args.GetText("eventName"), Script.Name);
    }
}