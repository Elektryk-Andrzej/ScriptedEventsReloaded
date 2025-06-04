using System.Collections.Generic;
using System.Linq;
using LabApi.Features.Wrappers;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

/// <summary>
/// Represents an argument that operates on a collection of items.
/// </summary>
public class ItemsArgument(string name) : BaseMethodArgument(name)
{
    public override OperatingValue Input =>
        OperatingValue.ItemType | OperatingValue.ItemReference | OperatingValue.ItemReferences |
        OperatingValue.AllOfType;

    public override string? AdditionalDescription => null;
    
    public ArgumentEvaluation<Item[]> GetConvertSolution(BaseToken token)
    {
        return DefaultConvertSolution<Item[]>(token, new()
        {
            [OperatingValue.ItemType] = itemType => Item.GetAll((ItemType)itemType).ToArray(),
            [OperatingValue.ItemReference] = item => new[] { (Item)item },
            [OperatingValue.ItemReferences] = items => ((IEnumerable<Item>)items).ToArray(),
            [OperatingValue.AllOfType] = _ => Item.List.ToArray()
        });
    }
}