using System.Collections.Generic;

namespace SER.Helpers.Extensions;

public static class DictionaryExtensions
{
    public static void AddOrInitListWithKey<TKey, TCollection, TCollectionValue>(
        this Dictionary<TKey, TCollection> dictionary, 
        TKey key, 
        TCollectionValue value
    ) where TCollection : List<TCollectionValue>, new()
    {
        if (dictionary.TryGetValue(key, out var list))
        {
            list.Add(value);
        }
        else
        {
            dictionary[key] = [value];
        }
    }
}