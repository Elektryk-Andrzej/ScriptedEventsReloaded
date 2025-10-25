using System.Linq;
using JetBrains.Annotations;
using LabApi.Features.Enums;
using LabApi.Features.Wrappers;
using MapGeneration;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;
using SER.TokenSystem.Tokens.Interfaces;
using SER.ValueSystem;

namespace SER.ArgumentSystem.Arguments;

public class DoorsArgument(string name) : EnumHandlingArgument(name)
{
    public override string InputDescription => $"{nameof(DoorName)} enum, {nameof(FacilityZone)} enum, {nameof(RoomName)} enum, reference to {nameof(Door)}, reference to {nameof(Room)} or * for every door";

    [UsedImplicitly]
    public DynamicTryGet<Door[]> GetConvertSolution(BaseToken token)
    {
        return ResolveEnums<Door[]>(
            token,
            new()
            {
                [typeof(DoorName)] = doorName =>
                    Door.List.Where(door => door.DoorName == (DoorName)doorName).ToArray(),
                
                [typeof(FacilityZone)] = zone => 
                    Door.List.Where(d => d.Zone == (FacilityZone)zone).Where(d => d is not ElevatorDoor).ToArray(),
                
                [typeof(RoomName)] = roomName => 
                    Door.List.Where(d => d.Rooms.Any(r => r.Name == (RoomName)roomName))
                        .Distinct().Where(d => d is not ElevatorDoor).ToArray(),
            },
            () =>
            {
                Result rs =
                    $"Value '{token.RawRep}' cannot be interpreted as a door or collection of doors.";
                if (token.RawRep == "*")
                {
                    return Door.List.Where(d => d is not ElevatorDoor).ToArray();
                }

                if (token is not IValueCapableToken<ReferenceValue> refToken)
                {
                    return rs;
                }
                
                return new(() =>
                {
                    if (ReferenceArgument<Door>.TryParse(refToken).WasSuccessful(out var door))
                    {
                        return new[] { door };
                    }

                    if (ReferenceArgument<Room>.TryParse(refToken).WasSuccessful(out var room))
                    {
                        return room.Doors.ToArray();
                    }
                    
                    return rs;
                });
            }
        );
    }
}