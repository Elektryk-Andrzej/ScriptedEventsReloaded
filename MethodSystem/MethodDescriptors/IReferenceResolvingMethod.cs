using System;

namespace SER.MethodSystem.MethodDescriptors;

/// <summary>
/// A method which takes a reference as an input and returns information about it in a readable format.
/// </summary>
public interface IReferenceResolvingMethod
{
    public Type ReferenceType { get; }
}