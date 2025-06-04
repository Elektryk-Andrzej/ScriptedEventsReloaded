using Interactables.Interobjects.DoorUtils;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.DoorMethods;

public class LockDoorMethod : Method
{
    public override string Description => "Locks doors.";

    public override BaseMethodArgument[] ExpectedArguments { get; } = 
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