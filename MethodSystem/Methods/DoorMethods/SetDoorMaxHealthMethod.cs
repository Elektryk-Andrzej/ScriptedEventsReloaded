using Interactables.Interobjects.DoorUtils;
using LabApi.Features.Wrappers;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.DoorMethods;
public class SetDoorMaxHealthMethod : SynchronousMethod
{
    public override string Description => "Sets max health for specified doors if possible";

    public override Argument[] ExpectedArguments =>
    [
        new DoorsArgument("doors"),
        new FloatArgument("maxhealth")
    ];

    public override void Execute()
    {
        float MaxHP = Args.GetFloat("maxhealth");
        Door[] doors = Args.GetDoors("doors");
        foreach (Door door in doors)
        {
            if (door is BreakableDoor brek)
            {
                brek.MaxHealth = MaxHP;
            }
        }
    }
}
