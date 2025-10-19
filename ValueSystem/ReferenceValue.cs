using System;
using System.Collections.Generic;
using SER.Helpers.Extensions;
using SER.Helpers.ResultSystem;

namespace SER.ValueSystem;

public class ReferenceValue(object obj) : LiteralValue(obj)
{
    public string Reference
    {
        get => field ??= RegisterObject(Value);
    } = null!;
    
    private static readonly Dictionary<string, (object value, Type type)> StoredObjects = new(StringComparer.OrdinalIgnoreCase);

    private static string RegisterObject(object obj)
    {
        var type = obj.GetType();
        var key = $"<{type.GetAccurateName()} reference | ID {obj.GetHashCode()}>";
        StoredObjects[key] = (obj, type);
        return key;
    }

    public static TryGet<object> TryRetreiveObject(string key)
    {
        return StoredObjects.TryGetValue(key, out var storedObject) 
            ? storedObject.value
            : $"There is no object with reference '{key}'" ;
    }

    protected override string StringRep => Reference;
}