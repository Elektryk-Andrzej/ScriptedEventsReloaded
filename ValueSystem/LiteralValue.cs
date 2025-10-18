using System;
using SER.Helpers.Extensions;

namespace SER.ValueSystem;

public abstract class LiteralValue(object value)
{
    public object Value => value;
    public string ValueName => GetType().Name;
    
    public static LiteralValue GetValueFromObject(object obj)
    {
        return obj switch
        {
            bool b      => new BoolValue(b),
            byte n      => new NumberValue(n),
            sbyte n     => new NumberValue(n),
            short n     => new NumberValue(n),
            ushort n    => new NumberValue(n),
            int n       => new NumberValue(n),
            uint n      => new NumberValue(n),
            long n      => new NumberValue(n),
            ulong n     => new NumberValue(n),
            float n     => new NumberValue((decimal)n),
            double n    => new NumberValue((decimal)n),
            decimal n   => new NumberValue(n),
            string s    => new TextValue(s),
            TimeSpan t  => new DurationValue(t),
            _           => new ReferenceValue(obj),
        };
    }

    public override string ToString()
    {
        if (this is ReferenceValue refVal)
        {
            return refVal.Reference;
        }
        
        return Value.ToString();
    }
    
    public static string GetFriendlyName(Type type)
    {
        return type.Name.Replace("Value", "").LowerFirst();
    }
}

public abstract class LiteralValue<T>(T value) : LiteralValue(value) where T : notnull
{
    public new T Value => value;
}

