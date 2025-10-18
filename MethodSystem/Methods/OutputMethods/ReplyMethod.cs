using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.OutputMethods;

public class ReplyMethod : SynchronousMethod
{
    public override string Description => 
        "Sends a message to the place where the script was run from. Usually used for replying to the command sender.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new TextArgument("message")
    ];
    
    public override void Execute()
    {
        Script.Executor.Reply(Args.GetText("message"), Script);
    }
}