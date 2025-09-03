using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.ElevatorMethods;

public class SendElevatorMethod : SynchronousMethod
{
    public override string Description => "Sends elevators to the next floor.";

    public override GenericMethodArgument[] ExpectedArguments { get; } =
    [
        new ElevatorsArgument("elevators")
    ];
    
    public override void Execute()
    {
        var elevators = Args.GetElevators("elevators");
        elevators.ForEach(el => el.SendToNextFloor());
    }
}