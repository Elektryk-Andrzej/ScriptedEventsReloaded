using System;
using System.Collections.Generic;
using System.Linq;
using SER.Helpers;
using SER.Helpers.Exceptions;
using SER.Helpers.Extensions;
using SER.ScriptSystem.FlagSystem.Structures;
using SER.ScriptSystem.TokenSystem;
using SER.ScriptSystem.TokenSystem.Structures;
using SER.ScriptSystem.TokenSystem.Tokens;
using EventHandler = SER.ScriptSystem.EventSystem.EventHandler;

namespace SER.ScriptSystem.FlagSystem;

public static partial class ScriptFlagHandler
{   
    public record FlagInfo(string ScriptName, string[] InlineArguments);
    
    private static readonly Dictionary<Flag, List<FlagInfo>> ScriptsFlags = [];
    
    private static readonly Dictionary<string, List<FlagInfo>> CustomScriptsFlags = [];
    
    private static readonly Dictionary<Flag, Action<FlagInfo>> FlagActions = new()
    {
        [Flag.Event] = EventHandler.ConnectEvent,
        [Flag.Command] = AssignCommandToScript
    };

    private static readonly Dictionary<Flag, Dictionary<FlagArgument, Action<string[]>>> AllowedFlagArguments = new()
    {
        [Flag.Command] = new()
        {
            [FlagArgument.Arguments] = AddCommandArguments,
            [FlagArgument.ConsoleType] = AddCommandConsoleType
        }
    };

    private static FlagInfo? _lastFlagInfo;
    private static Flag? _lastFlag;

    internal static void Clear()
    {
        ScriptsFlags.Clear();
        CustomScriptsFlags.Clear();
        EventHandler.EventClear();
        CommandClear();
    }
    
    internal static void RegisterScript(List<ScriptLine> scriptLinesWithFlags, string scriptName)
    {
        foreach (var tokens in scriptLinesWithFlags.Select(scrLine => scrLine.Tokens))
        {
            var name = tokens.Skip(1).FirstOrDefault()?.GetValue();
            if (name is null)
            {
                Log.Warn(scriptName, "Name of flag is missing.");
                continue;
            }

            var args = tokens.Skip(2).Select(t => t.GetValue()).ToArray();
            var prefix = tokens.FirstOrDefault();
            switch (prefix)
            {
                case FlagToken:
                    HandleFlag(name, args, scriptName);
                    break;
                case FlagArgumentToken:
                    HandleFlagArgument(name, args, scriptName);
                    break;
                default:
                    throw new DeveloperFuckupException($"{prefix} not flag or flag arg");
            }
        }
    }

    private static void HandleFlagArgument(string name, string[] arguments, string scriptName)
    {
        if (_lastFlag is null)
        {
            Log.Warn(scriptName, "You cannot provide flag arguments if there is no SER flag above.");
            return;
        }

        if (!AllowedFlagArguments.TryGetValue(_lastFlag.Value, out var allowedFlagArguments))
        {
            Log.Warn(scriptName, $"Flag {_lastFlag.Value} does not accept any additional arguments.");
            return;
        }
        
        if (!Enum.TryParse(name, true, out FlagArgument arg))
        {
            Log.Warn(scriptName, $"There is no flag argument name called '{name}'");
            return;
        }

        if (!allowedFlagArguments.TryGetValue(arg, out var registerAction))
        {
            Log.Warn(scriptName, $"Flag {_lastFlag.Value} does not accept the {arg} argument.");
            return;
        }
        
        registerAction(arguments);
    }

    private static void HandleFlag(string name, string[] arguments, string scriptName)
    {
        _lastFlagInfo = new FlagInfo(scriptName, arguments);
        
        if (!Enum.TryParse(name, true, out Flag flag))
        {
            _lastFlag = null;
            CustomScriptsFlags.AddOrInitListWithKey(name, _lastFlagInfo);
            return;
        }
        
        _lastFlag = flag;
        ScriptsFlags.AddOrInitListWithKey(flag, _lastFlagInfo);
        
        FlagActions.TryGetValue(flag, out var action);
        action?.Invoke(_lastFlagInfo);
    }
    
    public static bool? RunScript(FlagInfo info, Action<Script>? beforeScriptRanAction = null)
    {
        if (Script.CreateByScriptName(info.ScriptName).HasErrored(out var err, out var script))
        {
            Log.Error(info.ScriptName, err);
            return null;
        }
        
        beforeScriptRanAction?.Invoke(script);
        return script.RunForEvent();
    }

    public static List<FlagInfo> GetCustomFlags(string flagName)
    {
        return CustomScriptsFlags.TryGetValue(flagName, out var info) 
            ? info
            : [];
    }

    public static void RunScriptsWithCustomFlag(string flagName)
    {
        foreach (var info in GetCustomFlags(flagName))
        {
            RunScript(info);
        }
    }
    
    internal static List<FlagInfo> GetFlags(Flag flag)
    {
        return ScriptsFlags.TryGetValue(flag, out var info) 
            ? info
            : [];
    }
}











