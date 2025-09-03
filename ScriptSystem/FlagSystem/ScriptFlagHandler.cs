using System;
using System.Collections.Generic;
using System.Linq;
using LabApi.Features.Console;
using SER.Helpers;
using SER.Helpers.Exceptions;
using SER.Helpers.Extensions;
using SER.ScriptSystem.FlagSystem.Structures;
using SER.ScriptSystem.Structures;
using SER.ScriptSystem.TokenSystem;
using SER.ScriptSystem.TokenSystem.Structures;
using SER.ScriptSystem.TokenSystem.Tokens;
using EventHandler = SER.ScriptSystem.EventSystem.EventHandler;

namespace SER.ScriptSystem.FlagSystem;

public static class ScriptFlagHandler
{
    private static readonly Dictionary<string, List<Flag>> ScriptsFlags = [];
    
    private static readonly Dictionary<string, List<(string scriptName, string[] arguments)>> CustomScriptsFlags = [];
    
    private static Flag? _currentFlag;

    internal static void Clear()
    {
        ScriptsFlags.Values.ForEachItem(script => script.ForEach(flag => flag.Unbind()));
        ScriptsFlags.Clear();
        CustomScriptsFlags.Clear();
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
    }

    private static void HandleFlagArgument(string argName, string[] arguments, string scriptName)
    {
        if (_currentFlag is null)
        {
            Log.Error(scriptName, "You cannot provide flag arguments if there is no SER defined flag above.");
            return;
        }
        
        if (!Enum.TryParse(argName, true, out FlagArgument arg))
        {
            Log.Error(scriptName, $"There is no flag argument called '{argName}'");
            return;
        }

        if (!_currentFlag.Arguments.TryGetValue(arg, out var registerAction))
        {
            Log.Error(scriptName, $"Flag {_currentFlag.Type} does not accept the '{arg}' argument.");
            return;
        }

        if (registerAction(arguments).HasErrored(out var error))
        {
            Log.Error(scriptName, $"Error while handling flag argument '{argName}' in flag '{_currentFlag.Type}': {error}");
        }
    }

    private static void HandleFlag(string name, string[] arguments, string scriptName)
    {
        _currentFlag?.Confirm();

        Logger.Info($"handling flag {name} with args [{arguments.JoinStrings(" ")}] in script {scriptName}");
        if (!Enum.TryParse(name, out FlagType flagType))
        {
            _currentFlag = null;
            Logger.Info("adding custom flag");
            CustomScriptsFlags.AddOrInitListWithKey(scriptName, (name, arguments));
            return;
        }
        
        Logger.Info($"adding normal flag {flagType}");
        _currentFlag = Flag.Get(flagType, scriptName);
        
        if (_currentFlag.TryBind(arguments).HasErrored(out var error))
        {
            _currentFlag = null;
            Log.Error(scriptName, $"Error while handling flag '{name}': {error}");
            return;
        }
        
        ScriptsFlags.AddOrInitListWithKey(scriptName, _currentFlag);
    }
}











