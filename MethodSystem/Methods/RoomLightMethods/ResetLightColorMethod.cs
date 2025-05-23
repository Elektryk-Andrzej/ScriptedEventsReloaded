using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;
using SER.Helpers;
using UnityEngine;

namespace SER.MethodSystem.Methods.RoomLightMethods;

public class ResetLightColorMethod : Method
{
    public override string Description => "Resets the light color for rooms.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
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