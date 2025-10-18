using Interactables.Interobjects.DoorUtils;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.DoorMethods;

public class UnlockDoorMethod : SynchronousMethod
{
    public override string Description => "Unlocks doors.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new DoorsArgument("doors")
    ];

    public override void Execute()
    {
        var doors = Args.GetDoors("doors");

        foreach (var door in doors)
        {
            door.Base.NetworkActiveLocks = (ushort)DoorLockReason.None;
            DoorEvents.TriggerAction(door.Base, DoorAction.Unlocked, null);
        }
    }
}