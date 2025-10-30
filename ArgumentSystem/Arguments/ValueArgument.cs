using JetBrains.Annotations;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.Extensions;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;
using SER.ValueSystem;

namespace SER.ArgumentSystem.Arguments;

public class ValueArgument<T>(string name) : Argument(name) where T : Value
{
    public override string InputDescription => $"a value of type {typeof(T).GetAccurateName()}";
    
    [UsedImplicitly]
    public DynamicTryGet<T> GetConvertSolution(BaseToken token)
    {
        if (!token.CanReturn<T>(out var get))
        {
            return $"Value '{token.RawRep}' cannot represent {InputDescription}";
        }
        
        return new(() => get());
    }
}