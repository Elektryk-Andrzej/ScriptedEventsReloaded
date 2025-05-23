using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.MethodDescriptors;

namespace SER.MethodSystem.Methods.VariableMethods;

public class SaveMethod : TextReturningMethod, IPureMethod
{
    public override string Description => "Returns the provided text, which can be saved to a variable.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
        new TextArgument("text")
    ];

    public override void Execute()
    {
        TextReturn = Args.GetText("text");
    }
}