using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;
using SER.Helpers;

namespace SER.MethodSystem.Methods.RoomLightMethods;

public class SetLightColorMethod : Method
{
    public override string Description => "Sets the light color for rooms.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
        new RoomsArgument("rooms"),
        new ColorArgument("color"),
    ];
    
    public override void Execute()
    {
        var rooms = Args.GetRooms("rooms");
        var color = Args.GetColor("color");
        
        rooms.ForEachItem(room => 
            room.AllLightControllers.ForEachItem(ctrl =>
                ctrl.OverrideLightsColor = color));
    }
}