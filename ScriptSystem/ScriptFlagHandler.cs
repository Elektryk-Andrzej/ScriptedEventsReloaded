using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using LabApi.Events;
using LabApi.Events.Arguments.Interfaces;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using LabApi.Loader;
using SER.Helpers;
using SER.MethodSystem.Exceptions;
using SER.ScriptSystem.TokenSystem;
using SER.ScriptSystem.TokenSystem.Structures;
using SER.VariableSystem;
using SER.VariableSystem.Structures;

namespace SER.ScriptSystem;

public static class ScriptFlagHandler
{
    private record ScriptFlagInfo(string ScriptName, string[] FlagArguments);
    private static readonly Dictionary<string, List<ScriptFlagInfo>> ScriptsWithCustomFlags = [];
    private static List<EventInfo> _bindedEvents = [];
    private static MethodInfo? _onNonArgumentedEvent;
    private static MethodInfo? _onArgumentedEvent;
    
    public static List<EventInfo> AvailableEvents = [];
    
    public static void Initialize()
    {
        ScriptsWithCustomFlags.Clear();
        
        AvailableEvents = typeof(PluginLoader).Assembly.GetTypes()
            .Where(t => t.FullName?.Equals($"LabApi.Events.Handlers.{t.Name}") is true)
            .Select(t => t.GetEvents(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).ToList())
            .SelectMany(t => t).ToList();
        
        _onNonArgumentedEvent = typeof(ScriptFlagHandler).GetMethod(
                                    nameof(OnNonArgumentedEvent), 
                                    BindingFlags.Static | BindingFlags.NonPublic) 
                                ?? throw new DeveloperFuckupException();
        
        _onArgumentedEvent = typeof(ScriptFlagHandler).GetMethod(
                                 nameof(OnArgumentedEvent), 
                                 BindingFlags.Static | BindingFlags.NonPublic) 
                             ?? throw new DeveloperFuckupException(); 
    }

    public static void ClearIndex()
    {
        ScriptsWithCustomFlags.Clear();
    }
    
    public static void RegisterScript(List<ScriptLine> scriptLinesWithFlags, string scriptName)
    {
        foreach (var tokens in scriptLinesWithFlags.Select(sl => sl.Tokens.Skip(1).ToList()))
        {
            var flagName = tokens.First().GetValue();
            Logger.Info($"Registering script '{scriptName}' with flag '{flagName}'");
            
            AddScriptToCustomFlag(flagName, scriptName, 
                tokens.Skip(1).Select(t => t.GetValue()).ToArray());
        }
    }

    public static void RunScriptsWithCustomFlag(string flag, EventArgs? evArgs = null, Action<Script>? beforeScriptExecuted = null)
    {
        foreach (var name in GetScriptNames(flag))
        {
            RunScriptInternal(name, flag, evArgs, beforeScriptExecuted);
        }
    }

    private static void RunScriptInternal(ScriptFlagInfo info, string flag, EventArgs? evArgs, Action<Script>? beforeScriptExecuted = null)
    {
        if (Script.CreateByScriptName(info.ScriptName).HasErrored(out var err, out var script))
        {
            Logger.Error($"Can't run script '{info.ScriptName}' with flag '{flag}'. {err}");
            return;
        }
        
        beforeScriptExecuted?.Invoke(script);
        
        Logger.Info($"Running script '{info.ScriptName}' with flag '{flag}'");
        var isAllowed = script.RunForEvent();
        if (!isAllowed.HasValue || evArgs is not ICancellableEvent cancellable)
        {
            return;
        }
        
        cancellable.IsAllowed = isAllowed.Value;
    }

    private static void AddScriptToCustomFlag(string flag, string scriptName, string[] flagArguments)
    {
        ConnectEvent(flag);
        
        ScriptsWithCustomFlags.AddOrInit(flag, new ScriptFlagInfo(scriptName, flagArguments));
    }
    
    private static List<ScriptFlagInfo> GetScriptNames(string customFlag)
    {
        return ScriptsWithCustomFlags.TryGetValue(customFlag, out var scriptNames) 
            ? scriptNames 
            : [];
    }

    private static void ConnectEvent(string eventName)
    {
        EventInfo? matchingEventInfo = AvailableEvents.FirstOrDefault(e => e.Name == eventName);
        if (matchingEventInfo == null)
        {
            Logger.Error($"Event '{eventName}' does not exist!");
            return;
        }

        var subscribeMethod = matchingEventInfo.GetAddMethod(false);
        var genericType = matchingEventInfo.EventHandlerType.GetGenericArguments().FirstOrDefault();
        if (genericType is null)
        {
            BindNonArgumented(subscribeMethod);
            return;
        }

        BindArgumented(subscribeMethod, genericType);
    }
    
    private static void BindNonArgumented(MethodInfo subscribeMethod)
    {
        if (_onNonArgumentedEvent is null)
        {
            throw new DeveloperFuckupException();
        }
        
        var handlerInfo = Delegate.CreateDelegate(
            typeof(LabEventHandler),
            null,
            _onNonArgumentedEvent);

        subscribeMethod.Invoke(null!, [handlerInfo]);
    }
    
    private static void BindArgumented(MethodInfo subscribeMethod, Type generic)
    {
        if (_onArgumentedEvent is null)
        {
            throw new DeveloperFuckupException();
        }
        
        var argumentedEventHandlerDelegate = Delegate.CreateDelegate(
            typeof(LabEventHandler<>).MakeGenericType(generic),
            null, 
            _onArgumentedEvent.MakeGenericMethod(generic));
        
        subscribeMethod.Invoke(
            null!,
            [argumentedEventHandlerDelegate]);
    }

    private static void OnNonArgumentedEvent()
    {
        var evName = new StackFrame(2).GetMethod().Name.Substring("on".Length);
        RunScriptsWithCustomFlag(evName);
    }

    private static void OnArgumentedEvent<T>(T ev) where T : EventArgs
    {
        var evName = new StackFrame(2).GetMethod().Name.Substring("on".Length);
        var variables = GetVariablesFromEvent(ev);

        RunScriptsWithCustomFlag(evName, ev, scr =>
        {
            scr.AddVariables(variables.ToArray());
        });
    }

    public static List<IVariable> GetMimicVariables(EventInfo ev)
    {
        var genericType = ev.EventHandlerType.GetGenericArguments().FirstOrDefault();
        if (genericType is null)
        {
            return [];
        }

        List<(Type, string)> properties = (
            from prop in genericType.GetProperties()
            let value = prop.PropertyType
            where value is not null
            select (value, prop.Name)
        ).ToList();
        
        return GetMimicVariablesForEventHelp(properties);
    }

    public static List<IVariable> GetVariablesFromEvent(EventArgs ev)
    {
        List<(object, string)> properties = (
            from prop in ev.GetType().GetProperties()
            let value = prop.GetValue(ev)
            where value is not null
            select (value, prop.Name)
        ).ToList();
        
        return InternalGetVariablesFromProperties(properties);
    }

    private static List<IVariable> InternalGetVariablesFromProperties(List<(object value, string name)> properties)
    {
        List<IVariable> variables = [];
        foreach (var (value, name) in properties)
        {
            switch (value)
            {
                case bool:
                case string:
                case int:
                case float:
                case double:
                case decimal:
                case byte:
                case sbyte:
                case short:
                case ushort:
                case uint:
                    variables.Add(new LiteralVariable
                    {
                        Name = GetName(),
                        Value = value.ToString
                    });
                    continue;
                case Enum enumValue:
                    variables.Add(new LiteralVariable
                    {
                        Name = GetName(),
                        Value = () => enumValue.ToString()
                    });
                    continue;
                case Player player:
                    variables.Add(new PlayerVariable
                    {
                        Name = GetName(),
                        Players = () => [player]
                    });
                    continue;
                case IEnumerable<Player> players:
                    variables.Add(new PlayerVariable
                    {
                        Name = GetName(),
                        Players = () => players.ToList()
                    });
                    continue;
                default:
                    var registeredObject = ObjectReferenceSystem.RegisterObject(value);

                    variables.Add(new ReferenceVariable
                    {
                        Name = GetName(),
                        // ReSharper disable once AccessToModifiedClosure
                        Value = () => registeredObject,
                        Type = value.GetType()
                    });
                    continue;
                
            }

            string GetName()
            {
                return $"ev{name.First().ToString().ToUpper()}{name.Substring(1)}";
            }
        }

        return variables;
    }
    
    private static List<IVariable> GetMimicVariablesForEventHelp(List<(Type type, string name)> properties)
    {
        List<IVariable> variables = [];
        foreach (var (type, name) in properties)
        {
            IVariable var = type switch
            {
                not null when type == typeof(bool) || type == typeof(string) || type == typeof(int) ||
                              type == typeof(float) || type == typeof(double) || type == typeof(decimal) ||
                              type == typeof(byte) || type == typeof(sbyte) || type == typeof(short) ||
                              type == typeof(ushort) ||
                              type == typeof(uint) => new LiteralVariable
                {
                    Name = GetName(),
                    Value = () => null!
                },
                not null when type.IsEnum => new LiteralVariable
                {
                    Name = GetName(),
                    Value = () => null!
                },
                not null when type == typeof(Player) => new PlayerVariable
                {
                    Name = GetName(),
                    Players = () => null!
                },
                not null when typeof(IEnumerable<Player>).IsAssignableFrom(type) => new PlayerVariable
                {
                    Name = GetName(),
                    Players = () => null!
                },
                _ => new ReferenceVariable
                {
                    Name = GetName(),
                    Value = () => null!,
                    Type = type!
                }
            };
            
            variables.Add(var);

            string GetName()
            {
                return $"ev{name.First().ToString().ToUpper()}{name.Substring(1)}";
            }
        }

        return variables;
    }
}











