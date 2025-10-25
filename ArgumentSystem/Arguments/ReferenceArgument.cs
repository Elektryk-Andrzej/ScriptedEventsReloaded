using JetBrains.Annotations;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.Extensions;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;
using SER.TokenSystem.Tokens.Interfaces;
using SER.ValueSystem;

namespace SER.ArgumentSystem.Arguments;

public class ReferenceArgument<TValue>(string name) : Argument(name)
{
    public override string InputDescription => $"A reference to {typeof(TValue).GetAccurateName()} object.";

    [UsedImplicitly]
    public DynamicTryGet<TValue> GetConvertSolution(BaseToken token)
    {
        if (token is not IValueCapableToken<ReferenceValue> refToken)
        {
            return $"Value '{token.RawRep}' does not represent a valid reference.";
        }

        return new(() => TryParse(refToken));
    }

    public static TryGet<TValue> TryParse(IValueCapableToken<ReferenceValue> token)
    {
        if (token.ExactValue.HasErrored(out var error, out var value))
        {
            return error;
        }

        if (value.Value is not TValue correctValue)
        {
            return $"The {value} reference is not compatible with {typeof(TValue).GetAccurateName()}.";
        }

        return correctValue;
    }
}