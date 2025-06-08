using System.Collections.Generic;
using System.Linq;
using LabApi.Features.Enums;
using LabApi.Features.Wrappers;
using MapGeneration;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

/// <summary>
/// Represents a door collection argument used in a method.
/// </summary>
public class DoorsArgument(string name) : BaseMethodArgument(name)
{
    public override OperatingValue Input => 
        OperatingValue.DoorName | OperatingValue.FacilityZone | OperatingValue.RoomName | OperatingValue.DoorReferences | 
        OperatingValue.RoomReferences | OperatingValue.AllOfType;

    public override string? AdditionalDescription => null;

    public ArgumentEvaluation<Door[]> GetConvertSolution(BaseToken token)
    {
        return DefaultConvertSolution<Door[]>(token, new()
        {
            [OperatingValue.DoorName] = doorName => 
                Door.List.Where(door => door.DoorName == (DoorName)doorName).ToArray(),
            
            [OperatingValue.FacilityZone] = zone => 
                Door.List.Where(d => d.Zone == (FacilityZone)zone).Where(d => d is not ElevatorDoor).ToArray(),
            
            [OperatingValue.RoomName] = roomName => 
                Door.List.Where(d => d.Rooms.Any(r => r.Name == (RoomName)roomName))
                    .Distinct().Where(d => d is not ElevatorDoor).ToArray(),
            
            [OperatingValue.DoorReferences] = doors => 
                ((IEnumerable<Door>)doors).ToArray(),
            
            [OperatingValue.RoomReferences] = rooms =>
                ((IEnumerable<Room>)rooms).ToList().SelectMany(r => r.Doors).Distinct().ToArray(),
            
            [OperatingValue.AllOfType] = _ => Door.List.Where(d => d is not ElevatorDoor).ToArray()
        });
    }
}