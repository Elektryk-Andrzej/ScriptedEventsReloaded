using System;
using JetBrains.Annotations;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.Exceptions;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;
using SER.ValueSystem;
using Result = SER.Helpers.ResultSystem.Result;

namespace SER.ArgumentSystem.Arguments;

public class DurationArgument(string name) : Argument(name)
{
    public override string InputDescription => "Duration in format #ms (milliseconds), #s (seconds), #m (minutes) etc., e.g. 5s or 2m";

    [UsedImplicitly]
    public DynamicTryGet<TimeSpan> GetConvertSolution(BaseToken token)
    {
        Result rs = $"Value '{token.RawRep}' is not a duration.";
        return token switch
        {
            DurationToken durToken => durToken.Value.Value,
            ExpressionToken { Type: MethodExpression methodExpr } => new(() =>
            {
                methodExpr.Method.Execute();
                if (methodExpr.Method.ReturnValue is not DurationValue durValue)
                {
                    throw new ScriptRuntimeError(rs);
                }

                return durValue.Value;
            }),
            _ => rs
        };
    }
}