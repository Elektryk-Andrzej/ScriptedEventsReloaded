using InventorySystem.Items.Usables.Scp330;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.ItemMethods;

public class GiveCandyMethod : SynchronousMethod
{
    public override string Description => "Gives candy to players.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("players"),
        new EnumArgument<CandyKindID>("candyType"),
        new IntArgument("amount", 1)
        {
            DefaultValue = 1
        }
    ];
    
    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var candyType = Args.GetEnum<CandyKindID>("candyType");
        var amount = Args.GetInt("amount");

        foreach (var plr in players)
        {
            for (int i = 0; i < amount; i++)
            {
                if (!Scp330Bag.TryGetBag(plr.ReferenceHub, out var bag))
                {
                    bag = plr.AddItem(ItemType.SCP330)?.Base as Scp330Bag;
                    if (!bag) continue;
                    bag.TryAddSpecific(candyType);
                    // removes the random candy given when adding the bag
                    bag.TryRemove(0);
                }
                else
                {
                    bag.TryAddSpecific(candyType);
                }
                
                bag.ServerRefreshBag();
            }
        }
    }
}