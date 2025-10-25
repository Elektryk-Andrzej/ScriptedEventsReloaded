﻿using SER.Helpers.ResultSystem;
using SER.ValueSystem;
using SER.VariableSystem.Bases;

namespace SER.VariableSystem.Variables;

public class LiteralVariable(string name, LiteralValue value) : Variable<LiteralValue>
{
    public override string Name => name;
    public override LiteralValue Value => value;

    public TryGet<T> TryGetValue<T>()
    {
        if (Value is T tValue)
        {
            return tValue;
        }

        return
            $"Variable '{Name}' is not a '{typeof(T).Name}' value variable, but a '{value.GetType().Name}' variable.";
    }
}

public class LiteralVariable<T>(string name, T value) : LiteralVariable(name, value)
    where T : LiteralValue
{
    public new T Value => (T)base.Value;
}