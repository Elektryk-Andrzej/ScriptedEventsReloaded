using System;
using System.Linq;
using LabApi.Features.Wrappers;
using MapGeneration;
using SER.Helpers.Extensions;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.MethodDescriptors;

namespace SER.MethodSystem.Methods.RoomMethods;

public class GetRoomByNameMethod : ReferenceReturningMethod, IAdditionalDescription
{
    public override Type ReturnType => typeof(Room);
    
    public override string Description => "Returns a reference to a room which has the provided name.";

    public string AdditionalDescription =>
        "If more than one room matches the provided name, a random room will be returned.";

    public override GenericMethodArgument[] ExpectedArguments { get; } =
    [
        new EnumArgument<RoomName>("room name")
    ];

    public override void Execute()
    {
        var roomName = Args.GetEnum<RoomName>("room name");
        ValueReturn = Room.List.Where(r => r.Name == roomName).GetRandomValue();
    }
}