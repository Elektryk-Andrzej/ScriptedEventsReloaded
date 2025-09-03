using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using SER.ScriptSystem.EventSystem;

namespace SER.MethodSystem.Methods.EventMethods;

public class EnableEventMethod : SynchronousMethod
{
    public override string Description => "Enables the provided event to run after being disabled.";

    public override GenericMethodArgument[] ExpectedArguments { get; } =
    [
        new TextArgument("eventName")
    ];
    
    public override void Execute()
    {
        EventHandler.EnableEvent(Args.GetText("eventName"));
    }
}