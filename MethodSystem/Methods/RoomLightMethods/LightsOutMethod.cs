using System.Linq;
using SER.Helpers;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.RoomLightMethods;

public class LightsOutMethod : Method
{
    public override string Description => "Turns off lights for rooms.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
        new RoomsArgument("rooms"),
        new DurationArgument("duration")
    ];
    
    public override void Execute()
    {
        var rooms = Args.GetRooms("rooms");
        var duration = Args.GetDuration("duration");
        
        foreach (var roomLightController in rooms.SelectMany(r => r.AllLightControllers))
        {
            roomLightController.FlickerLights(duration.ToFloatSeconds());
        }
    }
}