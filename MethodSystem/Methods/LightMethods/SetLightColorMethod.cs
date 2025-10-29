using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.Extensions;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.LightMethods;

public class SetLightColorMethod : SynchronousMethod
{
    public override string Description => "Sets the light color for rooms.";

    public override Argument[] ExpectedArguments { get; } =
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
                ctrl.OverrideLightsColor = color
            )
        );
    }
}