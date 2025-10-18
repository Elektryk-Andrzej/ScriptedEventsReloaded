using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.Extensions;
using SER.MethodSystem.BaseMethods;
using UnityEngine;

namespace SER.MethodSystem.Methods.LightMethods;

public class ResetLightColorMethod : SynchronousMethod
{
    public override string Description => "Resets the light color for rooms.";

    public override Argument[] ExpectedArguments { get; } =
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