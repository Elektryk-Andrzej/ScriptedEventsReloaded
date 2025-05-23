using Interactables.Interobjects.DoorUtils;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.DoorMethods;

public class UnlockDoorMethod : Method
{
    public override string Description => "Unlocks doors.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
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