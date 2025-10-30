using System;
using JetBrains.Annotations;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.Extensions;
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
        if (token is not IValueToken valueToken || valueToken.CanReturn<DurationValue>(out var get))
        {
            return $"Value '{token.RawRep}' is not a duration.";
        }

        if (valueToken.IsConstant)
        {
            return get().OnSuccess(v => v.ExactValue);
        }
        
        return new(() => get().OnSuccess(v => v.ExactValue));
    }
}