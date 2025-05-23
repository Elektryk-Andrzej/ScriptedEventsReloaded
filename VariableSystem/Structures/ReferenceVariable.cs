using System;

namespace SER.VariableSystem.Structures;

/// <summary>
/// Represents a reference variable that extends the functionality of the LiteralVariable class.
/// </summary>
/// <remarks>
/// This class introduces a required property to indicate the type of the reference variable.
/// It is designed to encapsulate information about variables with a reference type.
/// </remarks>
public class ReferenceVariable : LiteralVariable
{
    public required Type Type { get; init; }
}