using InventorySystem.Items.Usables.Scp330;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.ItemMethods;

public class GiveCandyMethod : SynchronousMethod
{
    public override string Description => "Gives candy to players.";

    public override GenericMethodArgument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("players"),
        new EnumArgument<CandyKindID>("candyType"),
        new IntArgument("amount", 1)
    ];
    
    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var candyType = Args.GetEnum<CandyKindID>("candyType");
        var amount = Args.GetIntAmount("amount");

        foreach (var plr in players)
        {
            for (int i = 0; i < amount; i++)
            {
                if (!Scp330Bag.TryGetBag(plr.ReferenceHub, out var bag))
                {
                    bag = plr.AddItem(ItemType.SCP330)?.Base as Scp330Bag;
                }
                
                if (bag == null) continue;
                
                bag.TryAddSpecific(candyType);
                bag.ServerRefreshBag();
            }
        }
    }
}