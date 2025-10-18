using System.Linq;
using JetBrains.Annotations;
using LabApi.Features.Enums;
using LabApi.Features.Wrappers;
using MapGeneration;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Tokens;

namespace SER.ArgumentSystem.Arguments;

public class DoorsArgument(string name) : EnumHandlingArgument(name)
{
    public override string InputDescription => $"{nameof(DoorName)} enum, {nameof(FacilityZone)} enum, {nameof(RoomName)} enum, reference to {nameof(Door)}, or * for every door";

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
                if (token.RawRepresentation == "*")
                {
                    return Door.List.Where(d => d is not ElevatorDoor).ToArray();
                }

                if (ReferenceArgument<Door>.TryParse(token, Script).WasSuccessful(out var door))
                {
                    return new[] { door };
                }
                
                return $"Value '{token.RawRepresentation}' cannot be interpreted as a door or collection of doors.";
            }
        );
    }
}