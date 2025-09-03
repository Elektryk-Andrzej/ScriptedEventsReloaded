using System;
using SER.Helpers.Extensions;
using SER.Plugin.Commands.HelpSystem;

namespace SER.MethodSystem.ArgumentSystem.Structures;

public record Option(string Value, string Description = "")
{
    public static implicit operator Option(string value)
    {
        return new Option(value);
    }

    public static Option Enum<T>(string value) where T : struct, Enum
    {
        HelpInfoStorage.UsedEnums.Add(typeof(T));
        return new(value, $"Returns a {typeof(T).GetAccurateName()} enum value");
    }
    
    public static Option Reference<T>(string value) where T : class
    {
        return new(value, $"Returns a reference to {typeof(T).GetAccurateName()} object");
    }
}