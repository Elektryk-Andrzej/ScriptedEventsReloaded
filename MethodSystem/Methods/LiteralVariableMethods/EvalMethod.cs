using SER.Helpers;
using SER.Helpers.Exceptions;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.MethodDescriptors;

namespace SER.MethodSystem.Methods.LiteralVariableMethods;

public class EvalMethod : TextReturningMethod, IPureMethod
{
    public override string Description => 
        "Evaluates the provided expression and returns the result. Used for math operations.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
        new TextArgument("value")
    ];

    public override void Execute()
    {
        var value = Args.GetText("value");
        if (ExpressionSystem.EvalString(value, Script).HasErrored(out var error, out var result))
        {
            throw new MalformedConditionException(error);
        }
        
        TextReturn = result;
    }
}