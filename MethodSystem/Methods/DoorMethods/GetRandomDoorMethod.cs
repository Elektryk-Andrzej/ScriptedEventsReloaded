using System;
using LabApi.Features.Wrappers;
using SER.Helpers.Extensions;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.MethodDescriptors;

namespace SER.MethodSystem.Methods.DoorMethods;

public class GetRandomDoorMethod : ReferenceReturningMethod, IPureMethod
{
    public override string Description => "Returns a reference to a random door.";
    
    public override Type ReturnType => typeof(Door);

    public override GenericMethodArgument[] ExpectedArguments { get; } = [];
    
    public override void Execute()
    {
        ValueReturn = Door.List.GetRandomValue()!;
    }
}