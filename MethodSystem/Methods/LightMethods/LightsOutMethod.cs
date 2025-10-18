using System.Linq;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.Extensions;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.LightMethods;

public class LightsOutMethod : SynchronousMethod
{
    public override string Description => "Turns off lights for rooms.";

    public override Argument[] ExpectedArguments { get; } =
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