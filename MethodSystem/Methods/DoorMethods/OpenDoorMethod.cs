using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.DoorMethods;

public class OpenDoorMethod : SynchronousMethod
{
    public override string Description => "Opens doors.";

    public override Argument[] ExpectedArguments { get; } =
    [       
        new DoorsArgument("doors")
    ];
    
    public override void Execute()
    {
        var doors = Args.GetDoors("doors");
        
        foreach (var door in doors)
        {
            door.IsOpened = true;
        }
    }
}