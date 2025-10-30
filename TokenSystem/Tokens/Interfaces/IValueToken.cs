using System;
using SER.Helpers.ResultSystem;
using SER.ValueSystem;

namespace SER.TokenSystem.Tokens.Interfaces;

public interface IValueToken
{
    /// <summary>
    /// Returns the value associated with the token.
    /// </summary>
    public TryGet<Value> Value();
    
    /// <summary>
    /// A signature of all possible return values. Null if return signature unknown.
    /// </summary>
    public Type[]? PossibleValueTypes { get; }
    
    /// <summary>
    /// Whether the value is a constant and can be cached.
    /// </summary>
    public bool IsConstant { get; }
}