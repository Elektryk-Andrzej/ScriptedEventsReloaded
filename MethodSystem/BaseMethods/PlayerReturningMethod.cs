using LabApi.Features.Wrappers;

namespace SER.MethodSystem.BaseMethods;

/// <summary>
/// Represents a standard method that returns an array of players.
/// </summary>
public abstract class PlayerReturningMethod : Method
{
    public Player[]? PlayerReturn { get; protected set; }
}