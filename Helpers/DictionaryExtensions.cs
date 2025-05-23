using System.Collections.Generic;

namespace SER.Helpers;

public static class DictionaryExtensions
{
    public static void AddOrInit<TKey, TCollection, TValue>(
        this Dictionary<TKey, TCollection> dictionary, 
        TKey key, 
        TValue value
    ) where TCollection : List<TValue>, new()
    {
        if (dictionary.ContainsKey(key))
        {
            dictionary[key].Add(value);
        }
        else
        {
            dictionary[key] = [value];
        }
    }
}