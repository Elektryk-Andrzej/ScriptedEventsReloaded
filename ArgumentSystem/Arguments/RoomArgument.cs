using System.Linq;
using JetBrains.Annotations;
using LabApi.Features.Wrappers;
using MapGeneration;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;

namespace SER.ArgumentSystem.Arguments;

public class RoomArgument(string name) : EnumHandlingArgument(name)
{
    public override string InputDescription => $"{nameof(RoomName)} enum or reference to {nameof(Room)}";

    [UsedImplicitly]
    public DynamicTryGet<Room> GetConvertSolution(BaseToken token)
    {
        return ResolveEnums<Room>(
            token,
            new()
            {
                [typeof(RoomName)] = roomName => Room.List.First(room => room.Name == (RoomName)roomName),
            },
            () =>
            {
                if (ReferenceArgument<Room>.TryParse(token, Script).WasSuccessful(out var room))
                {
                    return room;
                }

                return $"Value '{token.RawRepresentation}' cannot be interpreted as a room.";
            }
        );
    }
}