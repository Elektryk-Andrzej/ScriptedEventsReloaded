using System.Reflection;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.ElevatorMethods;

public class UnlockElevatorMethod : Method
{
    public override string Description => "Unlocks elevators.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
        new ElevatorsArgument("elevators")    
    ];
    
    public override void Execute()
    {
        var elevators = Args.GetElevators("elevators");
        elevators.ForEach(el => el.UnlockAllDoors());
    }
}