using System;
using JetBrains.Annotations;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;
using SER.TokenSystem.Tokens.Interfaces;
using SER.ValueSystem;

namespace SER.ArgumentSystem.Arguments;

public class DurationArgument(string name) : Argument(name)
{
    public override string InputDescription => "Duration in format #ms (milliseconds), #s (seconds), #m (minutes) etc., e.g. 5s or 2m";

    [UsedImplicitly]
    public DynamicTryGet<TimeSpan> GetConvertSolution(BaseToken token)
    {
        return token switch
        {
            DurationToken durToken => durToken.Value.ExactValue,
            IValueCapableToken<DurationValue> capable => new(() => capable.ExactValue.OnSuccess(v => v.ExactValue)),
            IValueCapableToken<LiteralValue> litCapable => new(() =>
            {
                if (litCapable.ExactValue.HasErrored(out var litValError, out var literalValue))
                {
                    return litValError;
                }

                if (literalValue.TryGetValue<DurationValue>().HasErrored(out var durValError, out var durationValue))
                {
                    return durValError;
                }
                
                return durationValue.ExactValue;
            }),
            _ => $"Value '{token.RawRep}' is not a duration."
        };
    }
}