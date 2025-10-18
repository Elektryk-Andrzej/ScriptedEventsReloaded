using System;

namespace SER.ValueSystem;

public class DurationValue(TimeSpan value) : LiteralValue<TimeSpan>(value)
{
    public static implicit operator DurationValue(TimeSpan value)
    {
        return new(value);
    }
    
    public static implicit operator TimeSpan(DurationValue value)
    {
        return value.Value;
    }
}