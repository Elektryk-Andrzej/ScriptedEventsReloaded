using System.Linq;
using SER.Helpers.Extensions;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.ItemMethods;

public class DropItemMethod : Method
{
    public override string Description => "Drops items from players' inventories.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("players"),
        new EnumArgument<ItemType>("itemTypeToDrop"),
        new IntArgument("amountToDrop", 1)
    ];
    
    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var itemTypeToDrop = Args.GetEnum<ItemType>("itemTypeToDrop");
        var amountToDrop = Args.GetIntAmount("amountToDrop");

        foreach (var plr in players)
        {
            plr.Items
                .Where(item => item.Type == itemTypeToDrop)
                .Take(amountToDrop)
                .ForEachItem(item => plr.DropItem(item));
        }
    }
}