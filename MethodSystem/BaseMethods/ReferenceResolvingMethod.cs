using System;
using SER.Helpers.Extensions;
using SER.MethodSystem.MethodDescriptors;

namespace SER.MethodSystem.BaseMethods;

public abstract class ReferenceResolvingMethod : TextReturningMethod, IPureMethod, IReferenceResolvingMethod
{
    public abstract Type ReferenceType { get; }
    public override string Description => $"Returns information about a given {ReferenceType.GetAccurateName()} reference.";
}