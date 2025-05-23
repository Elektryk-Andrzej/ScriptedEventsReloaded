using System.Collections.Generic;
using System.Linq;
using LabApi.Features.Console;
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
            [OperatingValue.DoorName] = doorName => Door.List.Where(door => door.DoorName == (DoorName)doorName).ToArray(),
            [OperatingValue.FacilityZone] = zone => Door.List.Where(d => d.Zone == (FacilityZone)zone).ToArray(),
            [OperatingValue.RoomName] = roomName => Door.List.Where(d => d.Rooms.Select(r => r.Name).Contains((RoomName)roomName)).ToArray(),
            [OperatingValue.DoorReferences] = doors => ((IEnumerable<Door>)doors).ToArray(),
            [OperatingValue.RoomReferences] = rooms => ((IEnumerable<Room>)rooms).ToList().SelectMany(r => r.Doors).ToArray(),
            [OperatingValue.AllOfType] = _ => Door.List.ToArray()
        });
    }
}