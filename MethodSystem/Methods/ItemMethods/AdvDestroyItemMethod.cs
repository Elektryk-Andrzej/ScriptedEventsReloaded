using LabApi.Features.Wrappers;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.ItemMethods;

public class AdvDestroyItemMethod : SynchronousMethod
{
    public override string Description => "Destroys items on the ground and in inventories.";

    public override Argument[] ExpectedArguments { get; } =
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