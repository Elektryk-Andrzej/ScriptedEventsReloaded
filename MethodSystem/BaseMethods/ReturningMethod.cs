using System;
using SER.ValueSystem;

namespace SER.MethodSystem.BaseMethods;

public abstract class ReturningMethod : SynchronousMethod 
{
    public Value? ReturnValue { get; protected set; }
    public abstract Type[]? ReturnTypes { get; }
}

public abstract class ReturningMethod<T> : ReturningMethod
    where T : Value
{
    public override Type[] ReturnTypes => [typeof(T)];

    public new T? ReturnValue
    {
        get => base.ReturnValue as T;
        protected set => base.ReturnValue = value;
    }
}