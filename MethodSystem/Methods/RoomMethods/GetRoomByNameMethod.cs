using System;
using System.Linq;
using LabApi.Features.Wrappers;
using MapGeneration;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.Exceptions;
using SER.Helpers.Extensions;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.MethodDescriptors;

namespace SER.MethodSystem.Methods.RoomMethods;

public class GetRoomByNameMethod : ReferenceReturningMethod, IAdditionalDescription
{
    public override Type ReturnType => typeof(Room);
    
    public override string Description => "Returns a reference to a room which has the provided name.";

    public string AdditionalDescription =>
        "If more than one room matches the provided name, a random room will be returned.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new EnumArgument<RoomName>("room name")
    ];

    public override void Execute()
    {
        var roomName = Args.GetEnum<RoomName>("room name");
        var room = Room.List.Where(r => r.Name == roomName).GetRandomValue();
        if (room is null)
        {
            throw new ScriptErrorException($"No room found with the provided name '{roomName}'.");
        }
        
        Reference = new(room);
    }
}