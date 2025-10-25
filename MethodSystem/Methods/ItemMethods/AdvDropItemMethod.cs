using System;
using LabApi.Features.Wrappers;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using SER.ValueSystem;

namespace SER.MethodSystem.Methods.ItemMethods;

public class AdvDropItemMethod : ReferenceReturningMethod
{
    public override string Description => 
        "Drops an item from player inventory and returns a reference to the pickup object of that item.";
    
    public override Type ReturnType => typeof(Pickup);

    public override Argument[] ExpectedArguments { get; } =
    [
        new ReferenceArgument<Item>("item")
    ];

    public override void Execute()
    {
        ReturnValue = new ReferenceValue(Args.GetReference<Item>("item").DropItem());
    }
}