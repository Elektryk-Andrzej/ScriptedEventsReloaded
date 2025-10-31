using JetBrains.Annotations;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.Extensions;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;
using SER.TokenSystem.Tokens.Interfaces;
using SER.ValueSystem;

namespace SER.ArgumentSystem.Arguments;

public class BoolArgument(string name) : Argument(name)
{
    public override string InputDescription => "boolean (true or false) value";

    public bool IsFunction { get; init; } = false;

    private static TryGet<bool> ParseAsLiteral(BaseToken token)
    {
        if (token.TryGetLiteralValue<BoolValue>().HasErrored(out var error, out var value))
        {
            return error;
        }

        return value.ExactValue;
    }
    
    [UsedImplicitly]
    public DynamicTryGet<bool> GetConvertSolution(BaseToken token)
    {
        Result error = $"Value '{token.RawRep}' cannot be interpreted as a boolean value or condition.";
        if (token is not IValueToken valueToken || !valueToken.CanReturn<BoolValue>(out var get))
        {
            return error;
        }

        return valueToken.IsConstant
            ? new(get().OnSuccess(v => v.ExactValue))
            : new(() => get().OnSuccess(v => v.ExactValue));
    }
}
