using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using LabApi.Events;
using LabApi.Events.Arguments.Interfaces;
using LabApi.Features.Wrappers;
using LabApi.Loader;
using SER.Helpers;
using SER.Helpers.Exceptions;
using SER.Helpers.Extensions;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem.Structures;
using SER.VariableSystem;
using SER.VariableSystem.Structures;

namespace SER.ScriptSystem.EventSystem;

public static class EventHandler
{
    private static readonly Dictionary<string, Action> UnsubscribeActions = [];
    private static readonly Dictionary<string, List<string>> ScriptsUsingEvent = [];
    private static readonly HashSet<string> DisabledEvents = [];
    private static MethodInfo? _onNonArgumentedEvent;
    private static MethodInfo? _onArgumentedEvent;
    public static List<EventInfo> AvailableEvents = [];
    
    internal static void Initialize()
    {
        AvailableEvents = typeof(PluginLoader).Assembly.GetTypes()
            .Where(t => t.FullName?.Equals($"LabApi.Events.Handlers.{t.Name}") is true)
            .Select(t => t.GetEvents(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public 
                                     | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).ToList())
            .Flatten().ToList();
        
        _onNonArgumentedEvent = typeof(EventHandler).GetMethod(
                                    nameof(OnNonArgumentedEvent), 
                                    BindingFlags.Static | BindingFlags.NonPublic) 
                                ?? throw new AndrzejFuckedUpException("non arg error");
        
        _onArgumentedEvent = typeof(EventHandler).GetMethod(
                                 nameof(OnArgumentedEvent), 
                                 BindingFlags.Static | BindingFlags.NonPublic) 
                             ?? throw new AndrzejFuckedUpException("arg error"); 
    }
    
    internal static void EventClear()
    {
        ScriptsUsingEvent.Clear();
        UnsubscribeActions.Values.ForEachItem(act => act());
        UnsubscribeActions.Clear();
        DisabledEvents.Clear();
    }

    internal static void DisableEvent(string evName, string scriptName)
    {
        DisabledEvents.Add(evName);
        ConnectEvent(evName, scriptName, false);
    }

    internal static void EnableEvent(string evName, bool unsubscribe = false)
    {
        DisabledEvents.Remove(evName);
        if (unsubscribe && UnsubscribeActions.TryGetValue(evName, out var action))
        {
            action();
        }
    }
    
    internal static Result ConnectEvent(string evName, string scriptName, bool allowNonArg = true) 
    {
        if (ScriptsUsingEvent.TryGetValue(evName, out var scriptsConnected))
        {
            scriptsConnected.Add(scriptName);
            return true;
        }
        
        ScriptsUsingEvent.Add(evName, [scriptName]);

        EventInfo? matchingEventInfo = AvailableEvents.FirstOrDefault(e => e.Name == evName);
        if (matchingEventInfo == null)
        {
            return $"Event '{evName}' does not exist!"; 
        }
        
        var genericType = matchingEventInfo.EventHandlerType.GetGenericArguments().FirstOrDefault();
        if (genericType is not null)
        {
            BindArgumented(matchingEventInfo, genericType);
            return true;
        }

        if (!allowNonArg)
        {
            return $"Event '{evName}' must be an argumented event!";
        }
        
        BindNonArgumented(matchingEventInfo);
        return true;
    }
    
    private static void BindNonArgumented(EventInfo eventInfo)
    {
        if (_onNonArgumentedEvent is null)
        {
            throw new AndrzejFuckedUpException();
        }
    
        var handlerToUse = Delegate.CreateDelegate(
            typeof(LabEventHandler),
            null,
            _onNonArgumentedEvent);

        eventInfo.GetAddMethod(false).Invoke(null!, [handlerToUse]);
        UnsubscribeActions.Add(eventInfo.Name, () => eventInfo.GetRemoveMethod(false).Invoke(null!, [handlerToUse]));
    }
    
    private static void BindArgumented(EventInfo eventInfo, Type generic)
    {
        if (_onArgumentedEvent is null)
        {
            throw new AndrzejFuckedUpException();
        }

        var handlerToUse = Delegate.CreateDelegate(
            typeof(LabEventHandler<>).MakeGenericType(generic),
            null,
            _onArgumentedEvent.MakeGenericMethod(generic));

        eventInfo.GetAddMethod(false).Invoke(null!, [handlerToUse]);
        UnsubscribeActions.Add(eventInfo.Name, () => eventInfo.GetRemoveMethod(false).Invoke(null!, [handlerToUse]));
    }

    private static void OnNonArgumentedEvent()
    {
        var evName = new StackFrame(2).GetMethod().Name.Substring("on".Length);

        if (ScriptsUsingEvent.TryGetValue(evName, out var scriptsConnected))
        {
            foreach (var scrName in scriptsConnected)
            {
                var rs = new ResultStacker($"Failed to run script '{scrName}' connected to event '{evName}'");
                if (Script.CreateByScriptName(scrName, ScriptExecutor.Get()).HasErrored(out var error, out var script))
                {
                    Log.Error(scrName, rs.Add(error));
                    continue;
                }
                
                script.Run();
            }
        }
    }

    private static void OnArgumentedEvent<T>(T ev) where T : EventArgs
    {
        var evName = new StackFrame(2).GetMethod().Name.Substring("on".Length);
        
        if (ev is ICancellableEvent cancellable && DisabledEvents.Contains(evName))
        {
            cancellable.IsAllowed = false;
            return;
        }
        
        var variables = GetVariablesFromEvent(ev);
        if (ScriptsUsingEvent.TryGetValue(evName, out var scriptsConnected))
        {
            foreach (var scrName in scriptsConnected)
            {
                var rs = new ResultStacker($"Failed to run script '{scrName}' connected to event '{evName}'");
                if (Script.CreateByScriptName(scrName, ScriptExecutor.Get()).HasErrored(out var error, out var script))
                {
                    Log.Error(scrName, rs.Add(error));
                    continue;
                }

                script.AddVariables(variables);
                var isAllowed = script.RunForEvent();
                if (isAllowed.HasValue && ev is ICancellableEvent cancellable1)
                {
                    cancellable1.IsAllowed = isAllowed.Value;
                }
            }
        }
    }
    
    public static IVariable[] GetVariablesFromEvent(EventArgs ev)
    {
        List<(object, string, Type)> properties = (
            from prop in ev.GetType().GetProperties()
            let value = prop.GetValue(ev)
            let type = prop.PropertyType
            select (value, prop.Name, type)
        ).ToList();
        
        return InternalGetVariablesFromProperties(properties);
    }
    
    internal static List<IVariable> GetMimicVariables(EventInfo ev)
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

    private static IVariable[] InternalGetVariablesFromProperties(List<(object value, string name, Type type)> properties)
    {
        List<IVariable> variables = [];
        foreach (var (value, name, type) in properties)
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
                    variables.Add(new PlayerVariable(GetName(), [player]));
                    continue;
                case IEnumerable<Player> players:
                    variables.Add(new PlayerVariable(GetName(), players.ToList()));
                    continue;
                case null:
                    if (type == typeof(Player))
                    {
                         variables.Add(new PlayerVariable(GetName(), []));
                    }
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

        return variables.ToArray();
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
                
                not null when type == typeof(Player) => 
                    new PlayerVariable(GetName(), null!),
                
                not null when typeof(IEnumerable<Player>).IsAssignableFrom(type) => 
                    new PlayerVariable(GetName(), null!),
                
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