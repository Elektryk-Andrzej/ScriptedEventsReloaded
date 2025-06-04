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

    public static void Warn(Script scr, object obj)
    {
        var ident = scr.CurrentLine == 0 ? "Compile warning" : $"Warning in line {scr.CurrentLine}";
        Logger.Raw($"[Script '{scr.Name}'] [{ident}] {obj}", ConsoleColor.Yellow);
    }
    
    public static void Warn(string scrName, object obj)
    {
        Logger.Raw($"[Script '{scrName}'] {obj}", ConsoleColor.Yellow);
    }
    
    public static void Error(Script scr, object obj)
    {
        var ident = scr.CurrentLine == 0 ? "Compile error" : $"Error in line {scr.CurrentLine}";
        Logger.Raw($"[Script '{scr.Name}'] [{ident}] {obj!.ToString()}", ConsoleColor.Red);
    }
    
    public static void Error(string scrName, string msg)
    {
        Logger.Raw($"[Script '{scrName}'] {msg}", ConsoleColor.Red);
    }
}