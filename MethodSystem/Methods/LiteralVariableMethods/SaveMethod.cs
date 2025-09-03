using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.MethodDescriptors;

namespace SER.MethodSystem.Methods.LiteralVariableMethods;

public class SaveMethod : TextReturningMethod, IPureMethod
{
    public override string Description => "Returns the provided text, which can be saved to a variable.";

    public override GenericMethodArgument[] ExpectedArguments { get; } =
    [
        new TextArgument("text")
    ];

    public override void Execute()
    {
        TextReturn = Args.GetText("text");
    }
}