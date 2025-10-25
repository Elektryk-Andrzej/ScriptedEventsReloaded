using JetBrains.Annotations;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.Extensions;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;
using SER.TokenSystem.Tokens.Interfaces;
using SER.ValueSystem;

namespace SER.ArgumentSystem.Arguments;

public class ValueArgument<T>(string name) : Argument(name) where T : Value
{
    public override string InputDescription => $"a value of type {typeof(T).GetAccurateName()}";
    
    [UsedImplicitly]
    public DynamicTryGet<T> GetConvertSolution(BaseToken token)
    {
        if (token is not IValueCapableToken<T> valToken)
        {
            return $"Value '{token.RawRep}' cannot represent {InputDescription}";
        }
        
        return new(() => valToken.ExactValue);
    }
}