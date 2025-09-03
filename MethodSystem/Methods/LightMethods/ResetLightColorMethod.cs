using SER.Helpers.Extensions;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using UnityEngine;

namespace SER.MethodSystem.Methods.LightMethods;

public class ResetLightColorMethod : SynchronousMethod
{
    public override string Description => "Resets the light color for rooms.";

    public override GenericMethodArgument[] ExpectedArguments { get; } =
    [
        new RoomsArgument("rooms")
    ];
    
    public override void Execute()
    {
        Args.GetRooms("rooms").ForEach(room => 
            room.AllLightControllers.ForEachItem(color => 
                color.OverrideLightsColor = Color.clear));
    }
}