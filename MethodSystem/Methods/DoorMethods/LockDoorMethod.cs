using Interactables.Interobjects.DoorUtils;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.DoorMethods;

public class LockDoorMethod : SynchronousMethod
{
    public override string Description => "Locks doors.";

    public override GenericMethodArgument[] ExpectedArguments { get; } = 
    [       
        new DoorsArgument("doors"),
        new EnumArgument<DoorLockReason>("lock") 
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