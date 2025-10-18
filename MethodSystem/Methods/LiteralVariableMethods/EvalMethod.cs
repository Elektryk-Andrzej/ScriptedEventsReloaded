using System;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers;
using SER.Helpers.Exceptions;
using SER.MethodSystem.BaseMethods;
using SER.ValueSystem;

namespace SER.MethodSystem.Methods.LiteralVariableMethods;

public class EvalMethod : ReturningMethod
{
    public override string Description => 
        "Evaluates the provided expression and returns the result. Used for math operations.";
    public override Type[]? ReturnTypes => null;

    public override Argument[] ExpectedArguments { get; } =
    [
        new TextArgument("value")
    ];

    public override void Execute()
    {
        var value = Args.GetText("value");
        if (ExpressionReslover.EvalString(value, Script).HasErrored(out var error, out var result))
        {
            throw new ScriptErrorException(error);
        }
        
        Value = LiteralValue.GetValueFromObject(result);
    }
}