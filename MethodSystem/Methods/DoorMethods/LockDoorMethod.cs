using Interactables.Interobjects.DoorUtils;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.DoorMethods;

public class LockDoorMethod : SynchronousMethod
{
    public override string Description => "Locks doors.";

    public override Argument[] ExpectedArguments { get; } = 
    [       
        new DoorsArgument("doors"),
        new EnumArgument<DoorLockReason>("lock")
        {
            DefaultValue = DoorLockReason.AdminCommand
        }
    ];
    
    public override void Execute()
    {
        var doors = Args.GetDoors("doors");
        var lockType = Args.GetEnum<DoorLockReason>("lock");
        
        foreach (var door in doors)
        {
            door.Lock(lockType, true);
        }
    }
}