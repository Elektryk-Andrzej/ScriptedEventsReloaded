using System.Collections.Generic;
using System.Linq;
using LabApi.Features.Wrappers;
using MapGeneration;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

/// <summary>
/// Represents a room collection argument used in a method.
/// </summary>
public class RoomsArgument(string name) : BaseMethodArgument(name)
{
    public override OperatingValue Input =>
        OperatingValue.RoomName | OperatingValue.FacilityZone | OperatingValue.RoomReferences | 
        OperatingValue.AllOfType;

    public override string? AdditionalDescription => null;

    public ArgumentEvaluation<Room[]> GetConvertSolution(BaseToken token)
    {
        return DefaultConvertSolution<Room[]>(token, new()
        {
            [OperatingValue.RoomName] = roomName => Room.List.Where(room => room.Name == (RoomName)roomName).ToArray(),
            [OperatingValue.FacilityZone] = zone => Room.List.Where(room => room.Zone == (FacilityZone)zone).ToArray(),
            [OperatingValue.RoomReferences] = rooms => ((IEnumerable<Room>)rooms).ToArray(),
            [OperatingValue.AllOfType] = _ => Room.List.ToArray()
        });
    }
}