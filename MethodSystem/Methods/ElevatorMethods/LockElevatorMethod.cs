using Interactables.Interobjects.DoorUtils;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.ElevatorMethods;

public class LockElevatorMethod : Method
{
    public override string Description => "Locks elevators.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
        new ElevatorsArgument("elevators"),
        new EnumArgument<DoorLockReason>("lockReason")
    ];
    
    public override void Execute()
    {
        var elevators = Args.GetElevators("elevators");
        var lockReason = Args.GetEnum<DoorLockReason>("lockReason");
        
        elevators.ForEach(el => el.Base.ServerLockAllDoors(lockReason, true));
    }
}