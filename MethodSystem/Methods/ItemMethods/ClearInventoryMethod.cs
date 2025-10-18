using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.ItemMethods;

public class ClearInventoryMethod : SynchronousMethod
{
    public override string Description => "Clears player inventory.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("players")
    ];
    
    public override void Execute()
    {
        foreach (var plr in Args.GetPlayers("players"))
        {
            plr.Inventory.UserInventory.ReserveAmmo.Clear();
            plr.Inventory.SendAmmoNextFrame = true;
            plr.ClearInventory();
        }
    }
}