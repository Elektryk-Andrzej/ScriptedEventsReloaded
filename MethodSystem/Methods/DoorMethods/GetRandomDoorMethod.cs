using System;
using LabApi.Features.Wrappers;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.Extensions;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.DoorMethods;

public class GetRandomDoorMethod : ReferenceReturningMethod
{
    public override string Description => "Returns a reference to a random door.";
    
    public override Type ReturnType => typeof(Door);

    public override Argument[] ExpectedArguments { get; } = [];
    
    public override void Execute()
    {
        Reference = new(Door.List.GetRandomValue()!);
    }
}