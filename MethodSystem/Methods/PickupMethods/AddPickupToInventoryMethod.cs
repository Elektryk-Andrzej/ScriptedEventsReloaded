﻿using LabApi.Features.Wrappers;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.MethodDescriptors;

namespace SER.MethodSystem.Methods.PickupMethods;

public class AddPickupToInventoryMethod : SynchronousMethod, IAdditionalDescription
{
    public override string Description => "Forces a pickup to be added to the player's inventory.";

    public string AdditionalDescription => "Pickup will not be added if the player's inventory is full.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new PlayerArgument("player"),
        new ReferenceArgument<Pickup>("pickup")
    ];

    public override void Execute()
    {
        var player = Args.GetPlayer("player");
        var pickup = Args.GetReference<Pickup>("pickup");

        if (!player.IsInventoryFull)
        {
            player.AddItem(pickup);
        }
    }
}