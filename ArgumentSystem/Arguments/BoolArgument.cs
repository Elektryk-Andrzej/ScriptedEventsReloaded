using JetBrains.Annotations;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Structures;
using SER.TokenSystem.Tokens;
using SER.ValueSystem;

namespace SER.ArgumentSystem.Arguments;

public class BoolArgument(string name) : Argument(name)
{
    public override string InputDescription => "boolean (true or false) value";

    [UsedImplicitly]
    public DynamicTryGet<bool> GetConvertSolution(BaseToken token)
    {
        if (token.TryGetValue<BoolValue>().WasSuccessful(out var boolValue))
        {
            Log.D(11.ToString());
            return boolValue.Value;
        }
            
        if (token is ILiteralValueToken literalValueToken)
        {
            Log.D(21.ToString());
            return new(() =>
            {
                if (literalValueToken.GetLiteralValue(Script).HasErrored(out var error, out var value))
                {
                    return error;
                }

                if (value.Value is not bool @bool)
                {
                    return $"Value '{value}' retreived from {token.RawRepresentation} is not a boolean.";
                }

                return @bool;
            });
        }
        
        return $"Value '{token.RawRepresentation}' cannot be interpreted as a boolean value or condition.";
    }
}