using System.Linq;
using JetBrains.Annotations;
using LabApi.Features.Wrappers;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;

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
                if (token is SymbolToken { IsJoker: true })
                {
                    return Item.List.ToArray();
                }
                
                if (ReferenceArgument<Item>.TryParse(token, Script).WasSuccessful(out var item))
                {
                    return new[] { item };
                }
                
                return $"Value '{token.RawRepresentation}' cannot be interpreted as an item or collection of items.";
            }
        );
    }
}