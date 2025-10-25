using JetBrains.Annotations;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;
using SER.TokenSystem.Tokens.Interfaces;
using SER.TokenSystem.Tokens.Variables;
using SER.ValueSystem;

namespace SER.ArgumentSystem.Arguments;

public class CollectionArgument(string name) : Argument(name)
{
    public override string InputDescription => $"A collection variable e.g. {CollectionVariableToken.Example}";
    
    [UsedImplicitly]
    public DynamicTryGet<CollectionValue> GetConvertSolution(BaseToken token)
    {
        if (token is not IValueCapableToken<CollectionValue> valueCapableToken)
        {
            return $"Value '{token.RawRep}' does not represent a collection";
        }

        return new(() => valueCapableToken.ExactValue);
    }
}