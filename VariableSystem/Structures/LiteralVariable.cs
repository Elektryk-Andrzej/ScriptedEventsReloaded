using System;

namespace SER.VariableSystem.Structures;

/// <summary>
/// Represents a variable with a literal value that implements the IVariable interface.
/// </summary>
public class LiteralVariable : IVariable
{
    public virtual required string Name { get; init; }
    public virtual required Func<string> Value { get; init; }
}