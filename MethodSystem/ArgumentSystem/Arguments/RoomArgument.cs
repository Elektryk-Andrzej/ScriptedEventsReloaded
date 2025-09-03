using System.Linq;
using JetBrains.Annotations;
using LabApi.Features.Wrappers;
using MapGeneration;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

/// <summary>
/// Represents a single room argument used in a method.
/// </summary>
public class RoomArgument(string name) : GenericMethodArgument(name)
{
    public override string? AdditionalDescription => null;
    
    [UsedImplicitly]
    public ArgumentEvaluation<Room> GetConvertSolution(BaseToken token)
    {
        return MultipleSolutionConvert<Room>(token, new()
        {
            [OperatingValue.RoomName] = roomName => Room.List.First(room => room.Name == (RoomName)roomName),
            [OperatingValue.RoomReference] = room => (Room)room,
        });
    }
}