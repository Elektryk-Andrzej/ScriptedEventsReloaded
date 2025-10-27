using System;
using SER.Helpers.Extensions;
using SER.Helpers.ResultSystem;

namespace SER.ValueSystem;

public abstract class LiteralValue(object value) : Value
{
    protected abstract string StringRep { get; }
    
    public object BaseValue => value;

    public override string ToString()
    {
        return StringRep;
    }

    public string FriendlyName => GetFriendlyName(GetType());
    
    public static string GetFriendlyName(Type type)
    {
        return type.Name.Replace("Value", "").LowerFirst();
    }

    public TryGet<T> TryGetValue<T>() where T : Value
    {
        if (this is T tValue)
        {
            return tValue;
        }
        
        return $"Value is not of type {typeof(T).AccurateName}, but {BaseValue.GetType().AccurateName}.";
    }
}

public abstract class LiteralValue<T>(T value) : LiteralValue(value) 
    where T : notnull
{
    public T ExactValue => value;
}

