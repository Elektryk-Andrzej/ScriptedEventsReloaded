using System.Linq;
using JetBrains.Annotations;
using LabApi.Features.Wrappers;
using MapGeneration;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;

namespace SER.ArgumentSystem.Arguments;

public class RoomsArgument(string name) : EnumHandlingArgument(name)
{
    public override string InputDescription => 
        $"{nameof(RoomName)} enum, {nameof(FacilityZone)} enum, reference to {nameof(Room)}, or * for every room";

    [UsedImplicitly]
    public DynamicTryGet<Room[]> GetConvertSolution(BaseToken token)
    {
        return ResolveEnums<Room[]>(
            token,
            new()
            {
                [typeof(RoomName)] = roomName => Room.List.Where(room => room.Name == (RoomName)roomName).ToArray(),
                [typeof(FacilityZone)] = zone => Room.List.Where(room => room.Zone == (FacilityZone)zone).ToArray(),
            },
            () =>
            {
                if (token is SymbolToken { IsJoker: true })
                {
                    return Room.List.ToArray();
                }

                if (ReferenceArgument<Room>.TryParse(token, Script).WasSuccessful(out var room))
                {
                    return new[] { room };
                }
                
                return $"Value '{token.RawRepresentation}' cannot be interpreted as a room or collection of rooms.";
            }
        );
    }
}