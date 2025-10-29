using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LabApi.Features.Console;
using SER.Helpers.Extensions;
using SER.Helpers.ResultSystem;

namespace SER.FlagSystem.Structures;

public abstract class Flag
{
    public abstract string Description { get; }

    public readonly struct Argument(string name, string description, Func<string[], Result> handler, bool required)
    {
        public string Name => name;
        public string Description => description;
        public Result AddArgument(string[] values) => handler(values);
        public bool IsRequired => required;
    }

    public abstract Argument? InlineArgument { get; }

    public abstract Argument[] Arguments { get; }

    public abstract void FinalizeFlag();

    public abstract void Unbind();

    protected string ScriptName { get; set; } = null!;

    public string Name { get; set; } = null!;
    
    public static Dictionary<string, Type> FlagInfos = [];

    internal static void RegisterFlags()
    {
        FlagInfos = GetRegisteredFlags(Assembly.GetExecutingAssembly());
    }

    // ReSharper disable once UnusedMember.Global
    public static void RegisterFlagsAsExternalPlugin()
    {
        Logger.Info($"Registering flags from '{Assembly.GetCallingAssembly().GetName().Name}' plugin.");
        var flags = GetRegisteredFlags(Assembly.GetCallingAssembly());
        FlagInfos = FlagInfos.Concat(flags).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    private static Dictionary<string, Type> GetRegisteredFlags(Assembly? ass = null)
    {
        ass ??= Assembly.GetExecutingAssembly();
        return ass.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(Flag).IsAssignableFrom(t))
            .ToDictionary(t => t.Name.Replace("Flag", ""), t => t);
    }

    public static TryGet<Flag> TryGet(string flagName, string scriptName)
    {
        if (!FlagInfos.TryGetValue(flagName, out var type))
        {
            return $"Flag '{flagName}' is not a valid flag.";
        }
        
        var flag = type.CreateInstance<Flag>();
        flag.ScriptName = scriptName;
        flag.Name = flagName;
        return flag;
    }
}