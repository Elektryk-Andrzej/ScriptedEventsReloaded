using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.OutputMethods;

public class ReplyMethod : SynchronousMethod
{
    public override string Description => 
        "Sends a message to the place where the script was run from. Usually used for replying to the command sender.";

    public override GenericMethodArgument[] ExpectedArguments { get; } =
    [
        new TextArgument("message")
    ];
    
    public override void Execute()
    {
        Script.Executor.Reply(Args.GetText("message"), Script);
    }
}