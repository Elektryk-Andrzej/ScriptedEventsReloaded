using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.ServerMethods;

public class ErrorMethod : SynchronousMethod
{
    public override string Description => "Sends an error message.";

    public override GenericMethodArgument[] ExpectedArguments { get; } =
    [
        new TextArgument("error")
    ];
    
    public override void Execute()
    {
        Script.Executor.Error(Args.GetText("error"), Script);
    }
}