using System;
using SER.Helpers.Extensions;

namespace SER.ValueSystem;

public abstract class LiteralValue(object value) : Value
{
    protected abstract string StringRep { get; }
    
    public object Value => value;

    public override string ToString()
    {
        return StringRep;
    }

    public string FriendlyName => GetFriendlyName(GetType());
    
    public static string GetFriendlyName(Type type)
    {
        return type.Name.Replace("Value", "").LowerFirst();
    }
}

public abstract class LiteralValue<T>(T value) : LiteralValue(value) 
    where T : notnull
{
    public new T Value => value;
}

