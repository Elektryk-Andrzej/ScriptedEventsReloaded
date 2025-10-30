using LabApi.Features.Wrappers;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.DoorMethods;
public class RepairDoorMethod : SynchronousMethod
{
    public override string Description => "Repairs specified doors if possible (for example, you can't repair Gate B, but you can repair normal HCZ doors)";

    public override Argument[] ExpectedArguments =>
    [
        new DoorsArgument("doors")
        {
            Description="Doors to repair"
        }
    ];

    public override void Execute()
    {
        Door[] doors = Args.GetDoors("doors");
        foreach(Door door in doors)
        {
            if(door is BreakableDoor brek)
            {
                brek.TryRepair();
            }
        }
    }
}
