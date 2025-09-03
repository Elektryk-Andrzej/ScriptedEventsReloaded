using System;
using LabApi.Features.Wrappers;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.ItemMethods;

public class AdvDropItemMethod : ReferenceReturningMethod
{
    public override string Description => 
        "Drops an item from player inventory and returns a reference to the pickup object of that item.";
    
    public override Type ReturnType => typeof(Pickup);

    public override GenericMethodArgument[] ExpectedArguments { get; } =
    [
        new ReferenceArgument<Item>("item")
    ];

    public override void Execute()
    {
        ValueReturn = Args.GetReference<Item>("item").DropItem();
    }
}