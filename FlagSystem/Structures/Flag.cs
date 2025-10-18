using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SER.Helpers.ResultSystem;

namespace SER.FlagSystem.Structures;

public abstract class Flag
{
    public abstract string Description { get; }

    public abstract (string argName, string description)? InlineArgDescription { get; }

    public abstract Dictionary<string, (string description, Func<string[], Result> handler)> Arguments { get; }

    public abstract Result TryBind(string[] inlineArgs);

    public abstract void Confirm();

    public abstract void Unbind();

    protected string ScriptName { get; set; } = null!;

    public string Name { get; set; } = null!;
    
    public static Dictionary<string, 
        (
            Type type,
            string description, 
            (string argName, string description)? inlineArgDescription, 
            Dictionary<string, string> argDescription
        )> FlagInfos = [];

    public static void RegisterFlags()
    {
        FlagInfos = Assembly.GetExecutingAssembly().GetTypes()
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