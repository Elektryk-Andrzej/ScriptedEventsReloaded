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
using SER.ScriptSystem.FlagSystem.Structures;
using SER.VariableSystem;
using SER.VariableSystem.Structures;

namespace SER.ScriptSystem.FlagSystem;

public partial class ScriptFlagHandler
{
    private static readonly List<Action> UnsubscribeActions = [];
    private static readonly HashSet<string> ConnectedEvents = [];
    private static MethodInfo? _onNonArgumentedEvent;
    private static MethodInfo? _onArgumentedEvent;
    public static List<EventInfo> AvailableEvents = [];
    
    internal static void EventInit()
    {
        Clear();
        
        AvailableEvents = typeof(PluginLoader).Assembly.GetTypes()
            .Where(t => t.FullName?.Equals($"LabApi.Events.Handlers.{t.Name}") is true)
            .Select(t => t.GetEvents(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public 
                                     | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).ToList())
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
    
    internal static void EventClear()
    {
        ConnectedEvents.Clear();
        UnsubscribeActions.ForEach(act => act());
        UnsubscribeActions.Clear();
    }
    
    private static void ConnectEvent(FlagInfo info)
    {
        if (info.InlineArguments.Length == 0)
        {
            Log.Warn(info.ScriptName, "Script is using the event flag, but has not specified the event name.");
            return;
        }
        
        var eventName = info.InlineArguments[0];
        
        if (!ConnectedEvents.Add(eventName))
        {
            return;
        }
        
        EventInfo? matchingEventInfo = AvailableEvents.FirstOrDefault(e => e.Name == eventName);
        if (matchingEventInfo == null)
        {
            Log.Error(info.ScriptName, $"Event '{eventName}' does not exist!");
            return;
        }
        
        var genericType = matchingEventInfo.EventHandlerType.GetGenericArguments().FirstOrDefault();
        if (genericType is null)
        {
            BindNonArgumented(matchingEventInfo);
            return;
        }

        BindArgumented(matchingEventInfo, genericType);
    }
    
    private static void BindNonArgumented(EventInfo eventInfo)
    {
        if (_onNonArgumentedEvent is null)
        {
            throw new DeveloperFuckupException();
        }
        
        var handler = Delegate.CreateDelegate(
            typeof(LabEventHandler),
            null,
            _onNonArgumentedEvent);

        eventInfo.GetAddMethod(false).Invoke(null!, [handler]);
        UnsubscribeActions.Add(() => eventInfo.GetRemoveMethod(false).Invoke(null!, [handler]));
    }
    
    private static void BindArgumented(EventInfo eventInfo, Type generic)
    {
        if (_onArgumentedEvent is null)
        {
            throw new DeveloperFuckupException();
        }
        
        var handler = Delegate.CreateDelegate(
            typeof(LabEventHandler<>).MakeGenericType(generic),
            null, 
            _onArgumentedEvent.MakeGenericMethod(generic));
        
        eventInfo.GetAddMethod(false).Invoke(null!, [handler]);
        UnsubscribeActions.Add(() => eventInfo.GetRemoveMethod(false).Invoke(null!, [handler]));
    }

    private static void OnNonArgumentedEvent()
    {
        var evName = new StackFrame(2).GetMethod().Name.Substring("on".Length);
        
        GetFlags(Flag.Event)
            .Where(info => info.InlineArguments.FirstOrDefault()?.ToLower() == evName.ToLower())
            .ForEachItem(info => RunScript(info));
    }

    private static void OnArgumentedEvent<T>(T ev) where T : EventArgs
    {
        var evName = new StackFrame(2).GetMethod().Name.Substring("on".Length);
        var variables = GetVariablesFromEvent(ev);
        
        GetFlags(Flag.Event)
            .Where(info => info.InlineArguments.FirstOrDefault()?.ToLower() == evName.ToLower())
            .ForEachItem(info =>
            {
                var runResult = RunScript(info, scr =>
                {
                    scr.AddVariables(variables.ToArray());
                });
                
                if (!runResult.HasValue) return;

                if (ev is ICancellableEvent cancellable)
                {
                    cancellable.IsAllowed = runResult.Value;
                }
            });
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
                    variables.Add(new PlayerVariable(GetName(), [player]));
                    continue;
                case IEnumerable<Player> players:
                    variables.Add(new PlayerVariable(GetName(), players.ToList()));
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