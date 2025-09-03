using System.Linq;
using JetBrains.Annotations;
using LabApi.Features.Wrappers;
using MapGeneration;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

/// <summary>
/// Represents a room collection argument used in a method.
/// </summary>
public class RoomsArgument(string name) : GenericMethodArgument(name)
{
    public override string? AdditionalDescription => null;

    [UsedImplicitly]
    public ArgumentEvaluation<Room[]> GetConvertSolution(BaseToken token)
    {
        return MultipleSolutionConvert<Room[]>(token, new()
        {
            [OperatingValue.RoomName] = roomName => Room.List.Where(room => room.Name == (RoomName)roomName).ToArray(),
            [OperatingValue.FacilityZone] = zone => Room.List.Where(room => room.Zone == (FacilityZone)zone).ToArray(),
            //[OperatingValue.RoomReferences] = rooms => ((IEnumerable<Room>)rooms).ToArray(),
            [OperatingValue.RoomReference] = room => new[] { (Room)room },
            [OperatingValue.AllOfType] = _ => Room.List.ToArray()
        });
    }
}