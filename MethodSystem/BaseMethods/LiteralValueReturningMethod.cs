using System;
using SER.ValueSystem;

namespace SER.MethodSystem.BaseMethods;

public abstract class LiteralValueReturningMethod : ReturningMethod<LiteralValue>
{
    public abstract Type[] LiteralReturnTypes { get; }
    public override Type[] ReturnTypes => LiteralReturnTypes;
}