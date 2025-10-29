using System;
using System.Linq;
using Interactables.Interobjects.DoorUtils;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.DoorMethods;

public class SetDoorPermissionMethod : SynchronousMethod
{
    public override string Description => "Sets door permissions.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new DoorsArgument("doors"),
        new EnumArgument<DoorPermissionFlags>("permissions")
        {
            ConsumesRemainingValues = true,
        }
    ];
    
    public override void Execute()
    {
        var doors = Args.GetDoors("doors");
        var permissions = Args
            .GetRemainingArguments<object, EnumArgument<DoorPermissionFlags>>("permissions")
            .Cast<DoorPermissionFlags>()
            .ToArray();
        
        doors.ForEach(door =>
        {
            door.Permissions = permissions.Aggregate((a, b) => a | b);
        });
    }
}