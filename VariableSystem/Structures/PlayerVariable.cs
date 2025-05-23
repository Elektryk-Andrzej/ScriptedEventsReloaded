using System;
using System.Collections.Generic;
using LabApi.Features.Wrappers;

namespace SER.VariableSystem.Structures;

/// <summary>
/// Represents a variable that defines a collection of players.
/// This variable is intended to be used within the context of scripts
/// to provide dynamic access to specific groups of players.
/// </summary>
public class PlayerVariable : IVariable
{
    public required string Name { get; init; }
    public required Func<List<Player>> Players { get; init; }
}