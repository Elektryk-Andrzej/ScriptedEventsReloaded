using JetBrains.Annotations;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Structures;
using SER.TokenSystem.Tokens;
using SER.ValueSystem;
using UnityEngine;

namespace SER.ArgumentSystem.Arguments;

public class CollectionArgument(string name) : Argument(name)
{
    public override string InputDescription => "A literal variable with a collection";
    
    [UsedImplicitly]
    public DynamicTryGet<CollectionValue> GetConvertSolution(BaseToken token)
    {
        if (token is not ILiteralValueToken litVarToken)
        {
            return $"Value '{token.RawRepresentation}' cannot represent a collection.";
        }

        return new(() =>
        {
            if (litVarToken.GetLiteralValue(Script).HasErrored(out var error, out var value))
            {
                return error;
            }

            if (value is not CollectionValue collection)
            {
                return $"Value '{value}' fetched from '{token.RawRepresentation}' is not a collection.";
            }
            
            return collection;
        });
    }
}