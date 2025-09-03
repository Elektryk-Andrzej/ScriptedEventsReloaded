using System.Linq;
using JetBrains.Annotations;
using LabApi.Features.Enums;
using LabApi.Features.Wrappers;
using MapGeneration;
using SER.Helpers.Extensions;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

/// <summary>
/// Represents a door collection argument used in a method.
/// </summary>
public class DoorArgument(string name) : GenericMethodArgument(name)
{
    public override string? AdditionalDescription => null;

    [UsedImplicitly]
    public ArgumentEvaluation<Door> GetConvertSolution(BaseToken token)
    {
        return MultipleSolutionConvert<Door>(token, new()
        {
            [OperatingValue.DoorName] = doorName =>
            {
                var door = Door.List.Where(door => door.DoorName == (DoorName)doorName).GetRandomValue();
                if (door is null)
                {
                    return $"Door with name '{doorName}' does not exist.";
                }

                return door;
            },
            
            [OperatingValue.FacilityZone] = zone =>
            {
                var door = Door.List.Where(d => d.Zone == (FacilityZone)zone)
                    .Where(d => d is not ElevatorDoor)
                    .GetRandomValue();
                if (door is null)
                {
                    return $"No doors in zone '{zone}' exist.";
                }

                return door;
            },
        
            [OperatingValue.RoomName] = roomName =>
            {
                var door = Door.List.Where(d => d.Rooms.Any(r => r.Name == (RoomName)roomName))
                    .Distinct().Where(d => d is not ElevatorDoor).GetRandomValue();
                if (door is null)
                {
                    return $"No doors in room '{roomName}' exist.";
                }

                return door;
            },
            
            [OperatingValue.DoorReference] = door => (Door)door,
            
            
            // [OperatingValue.DoorReferences] = doors => 
            //     ((IEnumerable<Door>)doors).ToArray(),
            //
            // [OperatingValue.RoomReferences] = rooms =>
            //     ((IEnumerable<Room>)rooms).ToList().SelectMany(r => r.Doors).Distinct().ToArray(),
        });
    }
}