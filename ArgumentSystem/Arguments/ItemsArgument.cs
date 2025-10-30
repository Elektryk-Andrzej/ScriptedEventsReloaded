using System.Linq;
using JetBrains.Annotations;
using LabApi.Features.Wrappers;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.Extensions;
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

                if (token is not IValueToken valToken || !valToken.CanReturn<ReferenceValue>(out var get))
                {
                    return rs;
                }

                return new(() =>
                {
                    if (get().HasErrored(out var error, out var refValue))
                    {
                        return error;
                    }
                    
                    if (ReferenceArgument<Item>.TryParse(refValue).WasSuccessful(out var item))
                    {
                        return new[] { item };
                    }

                    return rs;
                });
            }
        );
    }
}