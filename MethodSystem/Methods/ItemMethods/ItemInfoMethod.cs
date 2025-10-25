using System;
using InventorySystem.Items;
using LabApi.Features.Wrappers;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.ArgumentSystem.Structures;
using SER.Helpers.Exceptions;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.MethodDescriptors;
using SER.ValueSystem;

namespace SER.MethodSystem.Methods.ItemMethods;

public class ItemInfoMethod : ReturningMethod, IReferenceResolvingMethod
{
    public override string Description => null!;

    public override Type[] ReturnTypes => [typeof(TextValue), typeof(PlayerValue), typeof(BoolValue)];
    public Type ReferenceType => typeof(ItemBase);
    
    public override Argument[] ExpectedArguments { get; } =
    [
        new ReferenceArgument<ItemBase>("reference"),
        new OptionsArgument("property", 
            Option.Enum<ItemType>("type"),
            Option.Enum<ItemCategory>("category"),
            "owner",
            "isEquipped"
        )
    ];
    
    public override void Execute()
    {
        var item = Args.GetReference<ItemBase>("reference");
        ReturnValue = Args.GetOption("property") switch
        {
            "type" => new TextValue(item.ItemTypeId.ToString()),
            "category" => new TextValue(item.Category.ToString()),
            "owner" => new PlayerValue(Player.Get(item.Owner)),
            "isEquipped" => new BoolValue(item.IsEquipped),
            _ => throw new AndrzejFuckedUpException()
        };
    }
}