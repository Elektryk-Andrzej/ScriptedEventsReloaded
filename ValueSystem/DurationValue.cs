using System;
using StringBuilder = System.Text.StringBuilder;

namespace SER.ValueSystem;

public class DurationValue(TimeSpan value) : LiteralValue<TimeSpan>(value)
{
    public static implicit operator DurationValue(TimeSpan value)
    {
        return new(value);
    }
    
    public static implicit operator TimeSpan(DurationValue value)
    {
        return value.ExactValue;
    }

    protected override string StringRep
    {
        get
        {
            StringBuilder sb = new();
            if (ExactValue.Hours > 0)
            {
                sb.Append($"{ExactValue.Hours}h ");
            }

            if (ExactValue.Minutes > 0)
            {
                sb.Append($"{ExactValue.Minutes}m ");
            }

            if (ExactValue.Seconds > 0)
            {
                sb.Append($"{ExactValue.Seconds}s ");
            }

            if (ExactValue.Milliseconds > 0)
            {
                sb.Append($"{ExactValue.Milliseconds:D3}ms ");
            }
            
            return sb.Remove(sb.Length - 1, 1).ToString();
        }
    }
}