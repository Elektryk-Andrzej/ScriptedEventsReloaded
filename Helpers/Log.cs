using System;
using LabApi.Features.Console;
using SER.ScriptSystem;

namespace SER.Helpers;

public static class Log
{
    public static void Debug<T>(T obj)
    {
        //Log.Info($"Debug: {obj!.ToString()}");
    }

    public static void Warn<T>(Script scr, T obj)
    {
        var ident = scr.CurrentLine == 0 ? "Compile warning" : $"Warning in line {scr.CurrentLine}";
        Logger.Raw($"[Script '{scr.Name}'] [{ident}] {obj!.ToString()}", ConsoleColor.Yellow);
    }

    public static void Error<T>(Script scr, T obj)
    {
        var ident = scr.CurrentLine == 0 ? "Compile error" : $"Error in line {scr.CurrentLine}";
        Logger.Raw($"[Script '{scr.Name}'] [{ident}] {obj!.ToString()}", ConsoleColor.Red);
    }
}