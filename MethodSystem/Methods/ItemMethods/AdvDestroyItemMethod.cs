using LabApi.Features.Wrappers;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.ItemMethods;

public class AdvDestroyItemMethod : Method
{
    public override string Description => "Destroys items on the ground and in inventories.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
        new ItemsArgument("items")
    ];
    
    public override void Execute()
    {
        var items = Args.GetItems("items");

        foreach (var item in items)
        {
            item.CurrentOwner?.RemoveItem(item);
            Pickup.Get(item.Serial)?.Destroy();
        }
    }
}