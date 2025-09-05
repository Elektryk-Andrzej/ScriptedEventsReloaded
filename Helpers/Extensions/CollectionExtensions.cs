using System;
using System.Collections.Generic;
using System.Linq;

namespace SER.Helpers.Extensions;

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

    public static IEnumerable<TResult> Flatten<TResult>(
        this IEnumerable<IEnumerable<TResult>> source)
    {
        return source.SelectMany(x => x);
    }

    public static string JoinStrings(this IEnumerable<string> source, string separator = "")
    {
        return string.Join(separator, source);
    }

    public static int Len<T>(this List<T> list)
    {
        return list.Count;
    }
    
    public static int Len<T>(this T[] array)
    {
        return array.Length;
    }
}