using System;
using LabApi.Features.Wrappers;
using SER.Helpers.Exceptions;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.RoomMethods;

public class RoomInfoMethod : ReferenceResolvingMethod
{
    public override Type ReferenceType => typeof(Room);
    public override string Description => $"Returns information about a given room from a {nameof(Room)} reference.";

    public override GenericMethodArgument[] ExpectedArguments { get; } =
    [
        new ReferenceArgument<Room>("room"),
        new OptionsArgument("property",
            "shape",
            "name",
            "zone",
            "pos",
            "xPos",
            "yPos",
            "zPos")
    ];
    
    public override void Execute()
    {
        var room = Args.GetReference<Room>("room");
        TextReturn = Args.GetOption("property") switch
        {
            "shape" => room.Shape.ToString(),
            "name" => room.Name.ToString(),
            "zone" => room.Zone.ToString(),
            "pos" => room.Position.ToString(),
            "xpos" => room.Position.x.ToString(),
            "ypos" => room.Position.y.ToString(),
            "zpos" => room.Position.z.ToString(),
            _ => throw new AndrzejFuckedUpException("room info property out of range")
        };
    }
}