using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LabApi.Features.Console;
using SER.Helpers.ResultSystem;
using FlagDictionary = System.Collections.Generic.Dictionary<string, 
    (
    System.Type type,
    string description, 
    (string argName, string description)? inlineArgDescription, System.Collections.Generic.Dictionary<string, string> argDescription
    )>;

namespace SER.FlagSystem.Structures;

public abstract class Flag
{
    public abstract string Description { get; }

    public abstract (string argName, string description)? InlineArgDescription { get; }

    public abstract Dictionary<string, (string description, Func<string[], Result> handler)> Arguments { get; }

    public abstract Result TryInitialize(string[] inlineArgs);

    public abstract void FinalizeFlag();

    public abstract void Unbind();

    protected string ScriptName { get; set; } = null!;

    public string Name { get; set; } = null!;
    
    public static FlagDictionary FlagInfos = [];

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

    private static FlagDictionary GetRegisteredFlags(Assembly ass)
    {
        return ass.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(Flag).IsAssignableFrom(t))
            .Select(t => (t, Activator.CreateInstance(t) as Flag))
            .Cast<(Type type, Flag flag)>()
            .ToDictionary(tuple => tuple.type.Name.Replace("Flag", ""), tuple =>
            {
                return
                (
                    tuple.type,
                    tuple.flag.Description,
                    tuple.flag.InlineArgDescription,
                    tuple.flag.Arguments.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.description)
                );
            });
    }

    public static TryGet<Flag> TryGet(string flagName, string scriptName)
    {
        if (!FlagInfos.TryGetValue(flagName, out var tuple))
        {
            return $"Flag '{flagName}' is not a valid flag.";
        }
        
        var flag = (Flag)Activator.CreateInstance(tuple.type);
        flag.ScriptName = scriptName;
        flag.Name = flagName;
        return flag;
    }
}