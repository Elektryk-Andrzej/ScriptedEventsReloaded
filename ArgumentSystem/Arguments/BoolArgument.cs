using JetBrains.Annotations;
using SER.ArgumentSystem.BaseArguments;
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
        if (ParseAsLiteral(token).WasSuccessful(out var value))
        {
            if (IsFunction) return new(() => ParseAsLiteral(token));
            return value;
        }
            
        if (token is IValueCapableToken<LiteralValue> literalValueToken)
        {
            return new(() =>
            {
                if (literalValueToken.ExactValue.HasErrored(out var error, out var valueTheGreat))
                {
                    return error;
                }

                if (valueTheGreat.BaseValue is not bool @bool)
                {
                    return $"Value '{valueTheGreat}' retreived from {token.RawRep} is not a boolean.";
                }

                return @bool;
            });
        }
        
        return $"Value '{token.RawRep}' cannot be interpreted as a boolean value or condition.";
    }
}
