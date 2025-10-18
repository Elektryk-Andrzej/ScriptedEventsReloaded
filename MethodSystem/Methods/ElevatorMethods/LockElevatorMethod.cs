using Interactables.Interobjects.DoorUtils;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.ElevatorMethods;

public class LockElevatorMethod : SynchronousMethod
{
    public override string Description => "Locks elevators.";

    public override Argument[] ExpectedArguments { get; } =
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