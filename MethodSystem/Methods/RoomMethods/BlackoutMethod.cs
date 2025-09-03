using System;
using System.Linq;
using Interactables.Interobjects.DoorUtils;
using MEC;
using SER.Helpers.Extensions;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.RoomMethods;

public class BlackoutMethod : SynchronousMethod
{
    public override string Description => "Blackouts rooms.";

    public override GenericMethodArgument[] ExpectedArguments { get; } =
    [
        new RoomsArgument("rooms"),
        new DurationArgument("duration")
        {
            DefaultValue = TimeSpan.MaxValue
        }
    ];
    
    public override void Execute()
    {
        var rooms = Args.GetRooms("rooms");
        var duration = Args.GetDuration("duration");
        
        var actualDuration = duration != TimeSpan.MaxValue
            ? duration.ToFloatSeconds()
            : -1;
        
        rooms.ForEach(room =>
        {
            foreach (var roomDoor in room.Doors)
            {
                roomDoor.Lock(DoorLockReason.Regular079, true);
                roomDoor.IsOpened = false;
            }

            foreach (var roomLightController in room.AllLightControllers)
            {
                roomLightController.FlickerLights(actualDuration);
            }
        });

        Timing.CallDelayed(actualDuration, () =>
        {
            foreach (var roomDoor in rooms.SelectMany(r => r.Doors))
            {
                roomDoor.IsLocked = false;
            }
        });
    }
}