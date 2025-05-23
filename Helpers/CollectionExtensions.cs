using System;
using System.Collections.Generic;
using System.Linq;

namespace SER.Helpers;

public static class CollectionExtensions
{
    public static void ForEachItem<T>(this IEnumerable<T> enumerable, Action<T> obj)
    {
        var list = enumerable as List<T> ?? enumerable.ToList();

        foreach (var value in list)
        {
            obj?.Invoke(value);
        }
    }
    
    public static T? GetRandomValue<T>(this IEnumerable<T> enumerable)
    {
        var array = enumerable.ToArray<T>();
        return array.Length != 0 ? array[UnityEngine.Random.Range(0, array.Length)] : default (T);
    }

    public static IEnumerable<T> RemoveNulls<T>(this IEnumerable<T?> enumerable)
    {
        return enumerable.Where(x => x != null)!;
    }
}