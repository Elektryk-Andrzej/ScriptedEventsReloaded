using System;
using SER.ValueSystem;

namespace SER.MethodSystem.BaseMethods;

/// <summary>
/// Represents a standard method that returns a text value.
/// </summary>
public abstract class ReturningMethod : SynchronousMethod
{
    public LiteralValue? Value { get; protected set; }
    public abstract Type[]? ReturnTypes { get; }
}