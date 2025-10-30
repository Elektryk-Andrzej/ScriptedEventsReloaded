﻿using System;
using System.Linq;
using SER.ScriptSystem;
using SER.ValueSystem;

namespace SER.TokenSystem.Tokens;

public class DurationToken : LiteralValueToken<DurationValue>
{
    protected override IParseResult InternalParse(Script scr)
    {
        var value = RawRep;
        if (TimeSpan.TryParse(value, out var result) && result.TotalMilliseconds > 0)
        {
            Value = result;
            return new Success();
        }

        var unitIndex = Array.FindIndex(value.ToCharArray(), char.IsLetter);
        if (unitIndex == -1)
        {
            return new Ignore();
        }

        var valuePart = value.Take(unitIndex).ToArray();
        if (!double.TryParse(string.Join("", valuePart), out var valueAsDouble))
        {
            return new Ignore();
        }
        
        if (valueAsDouble < 0)
        {
            return new Error("Duration cannot be negative.");
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
            return new Error($"Provided unit {unit} is not valid.");
        }

        Value = timeSpan.Value;
        return new Success();
    }
}