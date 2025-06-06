﻿using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.ItemMethods;

public class GiveItemMethod : Method
{
    public override string Description => "Gives an item to players.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
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
        var amount = Args.GetIntAmount("amount");

        foreach (var plr in players)
        {
            for (var i = 0; i < amount; i++)
            {
                plr.AddItem(item);
            }
        };
    }
}