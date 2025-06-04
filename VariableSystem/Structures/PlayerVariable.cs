using System.Collections.Generic;
using LabApi.Features.Wrappers;

namespace SER.VariableSystem.Structures;

/// <summary>
/// Represents a variable that defines a collection of players.
/// This variable is intended to be used within the context of scripts
/// to provide dynamic access to specific groups of players.
/// </summary>
public class PlayerVariable(string name, List<Player> players) : IVariable
{
    public virtual string Name => name;
    public virtual List<Player> Players => players;
}