using Interactables.Interobjects.DoorUtils;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.ElevatorMethods;

public class LockElevatorMethod : SynchronousMethod
{
    public override string Description => "Locks elevators.";

    public override GenericMethodArgument[] ExpectedArguments { get; } =
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