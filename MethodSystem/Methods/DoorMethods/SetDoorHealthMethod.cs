using LabApi.Features.Wrappers;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.DoorMethods;
public class SetDoorHealthMethod : SynchronousMethod
{
    public override string Description => "Sets remaining health for specified doors if possible (for example, you can't set health for Gate B, but you can set health for normal HCZ doors)";

    public override Argument[] ExpectedArguments =>
    [
        new DoorsArgument("doors"),
        new FloatArgument("health")
    ];

    public override void Execute()
    {
        float HP = Args.GetFloat("health");
        Door[] doors = Args.GetDoors("doors");
        foreach(Door door in doors)
        {
            if(door is BreakableDoor brek)
            {
                brek.Health = HP;
            }
        }
    }
}
