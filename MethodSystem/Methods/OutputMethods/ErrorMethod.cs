using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.OutputMethods;

public class ErrorMethod : SynchronousMethod
{
    public override string Description => "Sends an error message.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new TextArgument("error")
    ];
    
    public override void Execute()
    {
        Script.Executor.Error(Args.GetText("error"), Script);
    }
}