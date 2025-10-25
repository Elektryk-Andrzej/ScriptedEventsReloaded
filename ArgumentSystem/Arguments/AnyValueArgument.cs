using JetBrains.Annotations;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;
using SER.TokenSystem.Tokens.Interfaces;
using SER.ValueSystem;

namespace SER.ArgumentSystem.Arguments;

public class AnyValueArgument(string name) : Argument(name)
{
    public override string InputDescription => "Any value";
    
    [UsedImplicitly]
    public DynamicTryGet<Value> GetConvertSolution(BaseToken token)
    {
        if (token is IValueToken valToken)
        {
            return new(() => valToken.BaseValue);
        }
        
        return $"Value '{token.RawRep}' does not represent any kind of value";
    }
}