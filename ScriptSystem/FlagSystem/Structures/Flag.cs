using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SER.Helpers.ResultStructure;

namespace SER.ScriptSystem.FlagSystem.Structures;

public abstract class Flag
{
    public string ScriptName { get; set; } = null!;
    
    public abstract FlagType Type { get; }
    
    public abstract FlagArgument? InlineArgument { get; }
    
    public abstract Dictionary<FlagArgument, Func<string[], Result>> Arguments { get; }

    public abstract Result TryBind(string[] inlineArgs);

    public abstract void Confirm();
    
    public abstract void Unbind();

    private static Dictionary<FlagType, Type> _flags = [];

    public static void RegisterFlags()
    {
        _flags = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(Flag).IsAssignableFrom(t))
            .Select(t => (((Flag)Activator.CreateInstance(t)).Type, t))
            .ToDictionary(coll => coll.Type, coll => coll.t);
    }
    
    public static Flag Get(FlagType type, string scriptName)
    {
        var flag = (Flag)Activator.CreateInstance(_flags[type]);
        flag.ScriptName = scriptName;
        return flag;
    }
}