﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LabApi.Events;
using LabApi.Events.Arguments.Interfaces;
using LabApi.Features.Wrappers;
using LabApi.Loader;
using SER.Helpers;
using SER.Helpers.Extensions;
using SER.Helpers.ResultSystem;
using SER.ScriptSystem;
using SER.ScriptSystem.Structures;
using SER.ValueSystem;
using SER.VariableSystem.Variables;

namespace SER.EventSystem;

public static class EventHandler
{
    private static readonly Dictionary<string, Action> UnsubscribeActions = [];
    private static readonly Dictionary<string, List<string>> ScriptsUsingEvent = [];
    private static readonly HashSet<string> DisabledEvents = [];
    public static List<EventInfo> AvailableEvents = [];
    
    internal static void Initialize()
    {
        AvailableEvents = typeof(PluginLoader).Assembly.GetTypes()
            .Where(t => t.FullName?.Equals($"LabApi.Events.Handlers.{t.Name}") is true)
            .Select(t => t.GetEvents(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public 
                                     | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).ToList())
            .Flatten().ToList();
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
        var evName = eventInfo.Name;

        // Create delegate that captures evName
        LabEventHandler handler = () => OnNonArgumentedEvent(evName);

        // Subscribe
        eventInfo.GetAddMethod(false).Invoke(null!, [handler]);

        // Store unsubscribe action
        UnsubscribeActions[evName] = () => eventInfo.GetRemoveMethod(false).Invoke(null!, [handler]);
    }

    private static void BindArgumented(EventInfo eventInfo, Type generic)
    {
        var evName = eventInfo.Name;

        // We'll build (T ev) => OnArgumentedEvent(evName, ev)
        var evParam = Expression.Parameter(generic, "ev");
        var nameConst = Expression.Constant(evName);
        var call = Expression.Call(
            typeof(EventHandler)
                .GetMethod(nameof(OnArgumentedEvent), BindingFlags.Static | BindingFlags.NonPublic)!
                .MakeGenericMethod(generic),
            nameConst,
            evParam
        );

        // Compile delegate of correct type: LabEventHandler<T>
        var delegateType = typeof(LabEventHandler<>).MakeGenericType(generic);
        var lambda = Expression.Lambda(delegateType, call, evParam);
        var handler = lambda.Compile();

        // Subscribe
        eventInfo.GetAddMethod(false).Invoke(null!, [handler]);

        // Store unsubscribe action
        UnsubscribeActions[evName] = () => eventInfo.GetRemoveMethod(false).Invoke(null!, [handler]);
    }

    private static void OnNonArgumentedEvent(string evName)
    {
        Log.Debug($"[NonArg] Event '{evName}' triggered.");

        if (!ScriptsUsingEvent.TryGetValue(evName, out var scriptsConnected))
            return;

        foreach (var scrName in scriptsConnected)
        {
            Result rs = $"Failed to run script '{scrName}' connected to event '{evName}'";
            if (Script.CreateByScriptName(scrName, ScriptExecutor.Get()).HasErrored(out var error, out var script))
            {
                Log.Error(scrName, rs + error);
                continue;
            }

            script.Run();
        }
    }

    private static void OnArgumentedEvent<T>(string evName, T ev) where T : EventArgs
    {
        Log.Debug($"[Arg] Event '{evName}' triggered with {typeof(T).Name}.");

        if (ev is ICancellableEvent cancellable && DisabledEvents.Contains(evName))
        {
            cancellable.IsAllowed = false;
            Log.Debug($"Event '{evName}' cancelled (disabled).");
            return;
        }

        var variables = GetVariablesFromEvent(ev);
        if (!ScriptsUsingEvent.TryGetValue(evName, out var scriptsConnected))
        {
            Log.Debug($"Event '{evName}' has no scripts connected.");
            return;
        }

        foreach (var scrName in scriptsConnected)
        {
            Result rs = $"Failed to run script '{scrName}' connected to event '{evName}'";
            Log.Debug($"Running script '{scrName}' for event '{evName}'");
            if (Script.CreateByScriptName(scrName, ScriptExecutor.Get()).HasErrored(out var error, out var script))
            {
                Log.Error(scrName, rs + error);
                continue;
            }

            script.AddVariables(variables);
            var isAllowed = script.RunForEvent();
            if (isAllowed.HasValue && ev is ICancellableEvent cancellable1)
                cancellable1.IsAllowed = isAllowed.Value;
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
                case Enum enumValue:
                    variables.Add(new TextVariable(GetName(), enumValue.ToString()));
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
                {
                    variables.Add(LiteralVariable.CreateVariable(GetName(), LiteralValue.GetValueFromObject(value)));
                    continue;
                }
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
                              type == typeof(uint) => new TextVariable(GetName(), null!),
                
                not null when type.IsEnum => new TextVariable(GetName(), null!),
                
                not null when type == typeof(Player) => 
                    new PlayerVariable(GetName(), null!),
                
                not null when typeof(IEnumerable<Player>).IsAssignableFrom(type) => 
                    new PlayerVariable(GetName(), null!),
                
                _ => new ReferenceVariable(GetName(), null!)
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