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

    [UsedImplicitly]
    public DynamicTryGet<bool> GetConvertSolution(BaseToken token)
    {
        if (token.TryGetLiteralValue<BoolValue>().WasSuccessful(out var boolValue))
        {
            return boolValue.Value;
        }
            
        if (token is IValueCapableToken<LiteralValue> literalValueToken)
        {
            return new(() =>
            {
                if (literalValueToken.ExactValue.HasErrored(out var error, out var value))
                {
                    return error;
                }

                if (value.Value is not bool @bool)
                {
                    return $"Value '{value}' retreived from {token.RawRep} is not a boolean.";
                }

                return @bool;
            });
        }
        
        return $"Value '{token.RawRep}' cannot be interpreted as a boolean value or condition.";
    }
}