using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LabApi.Features.Console;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem;

public static class MethodIndex
{
    public static readonly Dictionary<string, BaseMethod> NameToMethodIndex = new();

    internal static void Initalize()
    {
        NameToMethodIndex.Clear();

        AddAllDefinedMethodsInAssembly();
    }

    public static void AddAllDefinedMethodsInAssembly()
    {
        var definedMethods = Assembly.GetCallingAssembly().GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(BaseMethod).IsAssignableFrom(t))
            .Select(t => Activator.CreateInstance(t) as BaseMethod);
        
        foreach (var method in definedMethods.OfType<BaseMethod>())
        {
            AddMethod(method);
        }
    }

    public static void AddMethod(BaseMethod method)
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