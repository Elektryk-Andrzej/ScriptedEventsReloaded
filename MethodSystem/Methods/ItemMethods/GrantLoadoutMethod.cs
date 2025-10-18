﻿using System.Collections.Generic;
using InventorySystem.Configs;
using PlayerRoles;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.ItemMethods;

public class GrantLoadoutMethod : SynchronousMethod
{
    public override string Description => "Grants players a class loadout.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("players"),
        new EnumArgument<RoleTypeId>("roleLoadout")
    ];
    
    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var role = Args.GetEnum<RoleTypeId>("roleLoadout");

        ItemType[] items;
        Dictionary<ItemType, ushort> ammo;
        if (StartingInventories.DefinedInventories.TryGetValue(role, out var inventoryInfo))
        {
            items = inventoryInfo.Items;
            ammo = inventoryInfo.Ammo;
        }
        else
        {
            items = [];
            ammo = [];
        }

        foreach (var player in players)
        {
            foreach (var item in items)
            {
                player.AddItem(item);
            }

            foreach (var kvp in ammo)
            {
                player.AddAmmo(kvp.Key, kvp.Value);
            }
        }
    }
}