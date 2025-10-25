using System.Linq;
using JetBrains.Annotations;
using LabApi.Features.Wrappers;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;
using SER.TokenSystem.Tokens.Interfaces;
using SER.ValueSystem;

namespace SER.ArgumentSystem.Arguments;

public class ItemsArgument(string name) : EnumHandlingArgument(name)
{
    public override string InputDescription => $"{nameof(ItemType)} enum, reference to {nameof(Item)}, or * for every item";

    [UsedImplicitly]
    public DynamicTryGet<Item[]> GetConvertSolution(BaseToken token)
    {
        return ResolveEnums<Item[]>(
            token,
            new()
            {
                [typeof(ItemType)] = itemType => Item.GetAll((ItemType)itemType).ToArray(),
            },
            () =>
            {
                Result rs = $"Value '{token.RawRep}' cannot be interpreted as {InputDescription}.";
                
                if (token is SymbolToken { IsJoker: true })
                {
                    return Item.List.ToArray();
                }

                if (token is not IValueCapableToken<ReferenceValue> refToken)
                {
                    return rs;
                }

                return new(() =>
                {
                    if (ReferenceArgument<Item>.TryParse(refToken).WasSuccessful(out var item))
                    {
                        return new[] { item };
                    }

                    return rs;
                });
            }
        );
    }
}