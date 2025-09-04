using System.Collections.Generic;
using System.Linq;
using LabApi.Features.Console;
using SER.Helpers;
using SER.Helpers.Exceptions;
using SER.Helpers.Extensions;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem.FlagSystem.Structures;
using SER.ScriptSystem.TokenSystem;
using SER.ScriptSystem.TokenSystem.Structures;
using SER.ScriptSystem.TokenSystem.Tokens;
using EventHandler = SER.ScriptSystem.EventSystem.EventHandler;

namespace SER.ScriptSystem.FlagSystem;

public static class ScriptFlagHandler
{
    private static readonly Dictionary<string, List<Flag>> ScriptsFlags = [];
    
    private static Flag? _currentFlag;

    internal static void Clear()
    {
        _currentFlag = null;
        ScriptsFlags.Values.ForEachItem(script => script.ForEach(flag => flag.Unbind()));
        ScriptsFlags.Clear();
        EventHandler.EventClear();
    }
    
    internal static void RegisterScript(List<ScriptLine> scriptLinesWithFlags, string scriptName)
    {
        Logger.Info($"handling flag lines in script {scriptName}");
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
                    throw new AndrzejFuckedUpException($"{prefix} not flag or flag arg");
            }
        }
        
        _currentFlag?.Confirm();
        _currentFlag = null;
    }

    private static void HandleFlagArgument(string argName, string[] arguments, string scriptName)
    {
        if (_currentFlag is null)
        {
            Log.Error(scriptName, "You cannot provide flag arguments if there is no valid SER flag above.");
            return;
        }

        if (!_currentFlag.Arguments.TryGetValue(argName, out var argInfo))
        {
            Log.Error(scriptName, $"Flag {_currentFlag.Name} does not accept the '{argName}' argument.");
            return;
        }

        if (argInfo.handler(arguments).HasErrored(out var error))
        {
            Log.Error(scriptName, $"Error while handling flag argument '{argName}' in flag '{_currentFlag.Name}': {error}");
        }
    }

    private static void HandleFlag(string name, string[] arguments, string scriptName)
    {
        _currentFlag?.Confirm();
        var rs = new ResultStacker($"Flag '{name}' failed when parsing.");

        Logger.Info($"handling flag {name} with args [{arguments.JoinStrings(" ")}] in script {scriptName}");
        if (Flag.TryGet(name, scriptName).HasErrored(out var getErr, out var flag))
        {
            Log.Error(scriptName, rs.Add(getErr));
            return;
        }
        
        if (flag.TryBind(arguments).HasErrored(out var bindErr))
        {
            Log.Error(scriptName, rs.Add(bindErr));
            return;
        }
        
        _currentFlag = flag;
        ScriptsFlags.AddOrInitListWithKey(scriptName, _currentFlag);
    }
}











