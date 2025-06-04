using LabApi.Features.Wrappers;
using SER.Helpers.Exceptions;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.MethodDescriptors;

namespace SER.MethodSystem.Methods.RoomMethods;

public class RoomInfoMethod : TextReturningMethod, IPureMethod
{
    public override string Description => $"Returns information about a given room from a {nameof(Room)} reference.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
        new ReferenceArgument<Room>("room"),
        new OptionsArgument("property",
            "shape",
            "name",
            "zone",
            "position")
    ];
    
    public override void Execute()
    {
        var room = Args.GetReference<Room>("room");
        TextReturn = Args.GetOption("property") switch
        {
            "shape" => room.Shape.ToString(),
            "name" => room.Name.ToString(),
            "zone" => room.Zone.ToString(),
            "position" => room.Position.ToString(),
            _ => throw new DeveloperFuckupException("room info property out of range")
        };
    }
}