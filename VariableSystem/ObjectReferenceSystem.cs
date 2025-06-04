using System;
using System.Collections.Generic;
using SER.Helpers;
using SER.Helpers.Extensions;

namespace SER.VariableSystem;

/// <summary>
///     Used when a value cannot be expressed with text, like a list, struct etc.
///     This doesn't include players, as there are player variables.
/// </summary>
public static class ObjectReferenceSystem
{
    private static readonly Dictionary<string, (object value, Type type)> StoredObjects = [];

    public static string RegisterObject(object obj)
    {
        var type = obj.GetType();
        var key = $"[{type.GetAccurateName()} reference | ID {obj.GetHashCode()}]";
        StoredObjects[key] = (obj, type);
        return key;
    }

    public static TryGet<object> TryRetreiveObject(string key)
    {
        return StoredObjects.TryGetValue(key, out var storedObject) 
            ? storedObject.value
            : $"There is no object with reference '{key}'" ;
    }
}