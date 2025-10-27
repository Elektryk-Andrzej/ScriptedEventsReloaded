using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.ItemMethods;

public class DestroyItemMethod : SynchronousMethod
{
    public override string Description => "Destroys an item in players' inventory.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("players"),
        new EnumArgument<ItemType>("item"),
        new IntArgument("amount", 1)
        {
            DefaultValue = 1
        }
    ];

    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var item = Args.GetEnum<ItemType>("item");
        var amount = Args.GetInt("amount");

        foreach (var plr in players)
        {
            plr.RemoveItem(item, amount);
        }
    }
}