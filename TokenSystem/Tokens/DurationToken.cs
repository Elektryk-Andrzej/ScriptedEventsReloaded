using System;
using System.Linq;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.ValueSystem;

namespace SER.TokenSystem.Tokens;

public class DurationToken : ValueToken<DurationValue>
{
    protected override Result InternalParse(Script scr)
    {
        var value = RawRepresentation;
        if (TimeSpan.TryParse(value, out var result) && result.TotalMilliseconds > 0)
        {
            Value = result;
            return true;
        }

        var unitIndex = Array.FindIndex(value.ToCharArray(), char.IsLetter);
        if (unitIndex == -1)
        {
            return "No unit provided.";
        }

        var valuePart = value.Take(unitIndex).ToArray();
        if (!double.TryParse(string.Join("", valuePart), out var valueAsDouble))
        {
            return $"Value part '{string.Join("", valuePart)}' is not a valid number.";
        }
        
        if (valueAsDouble < 0)
        {
            return "Duration cannot be negative.";
        }

        var unit = value.Substring(unitIndex);
        TimeSpan? timeSpan = unit switch
        {
            "s" => TimeSpan.FromSeconds(valueAsDouble),
            "ms" => TimeSpan.FromMilliseconds(valueAsDouble),
            "m" => TimeSpan.FromMinutes(valueAsDouble),
            "h" => TimeSpan.FromHours(valueAsDouble),
            "d" => TimeSpan.FromDays(valueAsDouble),
            _ => null
        };

        if (timeSpan is null)
        {
            return $"Provided unit {unit} is not valid.";
        }

        Value = timeSpan.Value;
        return true;
    }
}