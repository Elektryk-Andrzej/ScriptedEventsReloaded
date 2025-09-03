using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LabApi.Features.Console;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem;

public static class MethodIndex
{
    public static readonly Dictionary<string, Method> NameToMethodIndex = new();

    internal static void Initialize()
    {
        NameToMethodIndex.Clear();

        AddAllDefinedMethodsInAssembly();
    }

    public static void AddAllDefinedMethodsInAssembly()
    {
        var definedMethods = Assembly.GetCallingAssembly().GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(Method).IsAssignableFrom(t))
            .Select(t => Activator.CreateInstance(t) as Method)
            .ToList();
        
        foreach (var method in definedMethods.OfType<Method>())
        {
            AddMethod(method);
        }
        
        var assemblyName = Assembly.GetCallingAssembly().GetName().Name;
        Logger.Info($"'{assemblyName}' plugin has added {definedMethods.Count} methods.");
    }

    public static void AddMethod(Method method)
    {
        if (NameToMethodIndex.ContainsKey(method.Name))
        {
            Logger.Error($"Tried to register an already existing method '{method.Name}'!");
            return;
        }

        NameToMethodIndex.Add(method.Name, method);
    }

    internal static void Clear()
    {
        NameToMethodIndex.Clear();
    }
}