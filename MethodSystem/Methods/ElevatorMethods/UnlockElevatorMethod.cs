using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.ElevatorMethods;

public class UnlockElevatorMethod : SynchronousMethod
{
    public override string Description => "Unlocks elevators.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new ElevatorsArgument("elevators")    
    ];
    
    public override void Execute()
    {
        var elevators = Args.GetElevators("elevators");
        elevators.ForEach(el => el.UnlockAllDoors());
    }
}