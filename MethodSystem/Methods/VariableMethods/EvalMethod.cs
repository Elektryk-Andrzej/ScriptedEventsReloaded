using NCalc;
using SER.Helpers;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.Exceptions;
using SER.MethodSystem.MethodDescriptors;

namespace SER.MethodSystem.Methods.VariableMethods;

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
        if (Condition.TryEval(value, Script).HasErrored(out var error, out var result))
        {
            throw new MalformedConditionException(error);
        }
        
        TextReturn = result.ToString();
    }
}