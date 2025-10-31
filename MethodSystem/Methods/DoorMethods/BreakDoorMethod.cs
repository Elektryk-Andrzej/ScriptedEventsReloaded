using Interactables.Interobjects.DoorUtils;
using LabApi.Features.Wrappers;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.DoorMethods;
internal class BreakDoorMethod : SynchronousMethod
{
    public override string Description => "Breaks specified doors if possible (for example, you can't destroy Gate B, but you can destroy normal HCZ doors)";

    public override Argument[] ExpectedArguments => 
    [
        new DoorsArgument("doors")
        {
            Description="Doors to break"
        },
        new EnumArgument<DoorDamageType>("doordamagetype")
        {
            DefaultValue = DoorDamageType.ServerCommand,
            Description = "Type of damage to be applied on doors"
        }
    ];

    public override void Execute()
    {
        Door[] doors = Args.GetDoors("doors");
        foreach(Door door in doors)
        {
            if(door is BreakableDoor Brek)
            {
                Brek.TryBreak();
            }
        }
    }
}
