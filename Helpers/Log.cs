using System;
using LabApi.Features.Console;
using SER.Helpers.Exceptions;
using SER.ScriptSystem;

namespace SER.Helpers;

public static class Log
{
    public static void Debug<T>(T obj)
    {
        #if DEBUG
            Logger.Raw($"Debug: {obj!.ToString()}", ConsoleColor.Gray);
        #endif
    }

    public static void Warn(Script scr, object obj)
    {
        var ident = scr.CurrentLine == 0 ? "Compile warning" : $"Warning in line {scr.CurrentLine}";
        Logger.Raw($"[Script '{scr.Name}'] [{ident}] {obj}", ConsoleColor.Yellow);
    }
    
    public static void Warn(string scrName, object obj)
    {
        Logger.Raw($"[Script '{scrName}'] {obj}", ConsoleColor.Yellow);
    }
    
    public static void Error(string scrName, string msg)
    {
        Logger.Raw($"[Script '{scrName}'] {msg}", ConsoleColor.Red);
    }

    public static void Assert(bool condition, string msg)
    {
        if (!condition) throw new AndrzejFuckedUpException(msg);
    }

    public static void D(string msg)
    {
        #if DEBUG
            Logger.Raw(msg, ConsoleColor.Cyan);
        #endif
    }
}